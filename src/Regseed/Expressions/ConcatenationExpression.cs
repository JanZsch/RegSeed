using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Regseed.Common.Builder;
using Regseed.Common.Helper;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class ConcatenationExpression : BaseExpression
    {
        private readonly List<IExpression> _elementaryExpressions;

        public ConcatenationExpression(IRandomGenerator random) : base(random)
        {
            _elementaryExpressions = new List<IExpression>();
        }

        public ConcatenationExpression Append(IExpression expression)
        {
            _elementaryExpressions.Add(expression);
            return this;
        }
     
        public override IExpression GetInverse()
        {
            var seed = new List<List<IExpression>> {_elementaryExpressions};
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, null, WasExpandedRepeatExpressionAddedToList);

            var intersectInverseList = new List<IExpression>();

            var addGuard = new object();

            Parallel.ForEach(expandedRepeatRepresentation, representation =>
            {
                var inverse = GetInverseOfExpandedConcatRepresentation(representation);

                lock (addGuard)
                    intersectInverseList.Add(inverse);
            });

            return new IntersectionExpression(intersectInverseList, _random);
        }

        public override IExpression Clone()
        {
            var clone = new ConcatenationExpression(_random)
            {
                RepeatRange = RepeatRange?.Clone(),
                ExpansionLength = ExpansionLength
            };

            clone.AppendRange(_elementaryExpressions.Select(x => x.Clone()));

            return clone;
        }

        protected override IntegerInterval GetMaxExpansionInterval()
        {
            var maxExpansionLength = 0;
            var minExpansionLength = 0;
            
            foreach (var elementaryExpression in _elementaryExpressions)
            {
                elementaryExpression.MaxExpansionRange.ToExpansionBounds(out var baseMinLength, out var baseMaxLength);
                
                elementaryExpression.RepeatRange.ToExpansionBounds(out var minExpansionFactor, out var maxExpansionFactor);

                if (!IsExpansionFactorValid(baseMaxLength, maxExpansionFactor, maxExpansionLength))
                    maxExpansionLength = int.MaxValue;
                else
                    maxExpansionLength += maxExpansionFactor * baseMaxLength;
                
                if (!IsExpansionFactorValid(baseMinLength, minExpansionFactor, minExpansionLength))
                    minExpansionLength = int.MaxValue;
                else
                    minExpansionLength += minExpansionFactor * baseMinLength;
            }

            var maxExpansionInterval = new IntegerInterval();
            maxExpansionInterval.TrySetValue(minExpansionLength, maxExpansionLength);
            
            return maxExpansionInterval;
        }

        public override void SetExpansionLength(int expansionLength = 0)
        {
            ExpansionLength = expansionLength;

            MaxExpansionRange.ToLowerExpansionBound(out var totalLowerBound);

            if (totalLowerBound == 0)
            {
                foreach (var elementaryExpression in _elementaryExpressions)
                    elementaryExpression.SetExpansionLength(expansionLength);

                return;
            }
            
            foreach (var elementaryExpression in _elementaryExpressions)
            {
                elementaryExpression.RepeatRange.ToLowerExpansionBound(out var minimalExpansionFactor);
                elementaryExpression.MaxExpansionRange.ToLowerExpansionBound(out var minimalExpansionLength);

                var expansionLengthCorrection = totalLowerBound - minimalExpansionFactor * minimalExpansionLength;
                
                elementaryExpression.SetExpansionLength(expansionLength - expansionLengthCorrection);
            }
        }

        public override IList<IStringBuilder> Expand()
        {           
            var seed = new List<List<IExpression>> {_elementaryExpressions};
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, ExpansionLength, WasExpandedRepeatExpressionAddedToList);

            var convertedExpressions = new List<List<IList<IStringBuilder>>>();

            foreach (var expressionList in expandedRepeatRepresentation)
            {
                var expandedExpression = expressionList.Select(x => x.Expand()).ToList();
                convertedExpressions.Add(expandedExpression);
            }

            var expandedUnionList = ExpandHelper.ExpandListRepresentation(convertedExpressions, ExpansionLength, ExpandHelper.WasExpandedStringBuilderListAddedToList);

            return CreateConcatenatedStringBuilderForEachExpansion(expandedUnionList);
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            IStringBuilder builder = StringBuilder.Empty;

            foreach (var expression in _elementaryExpressions)
                builder = builder.ConcatWith(expression.ToStringBuilder());

            return builder;
        }
        
        private static IList<IStringBuilder> CreateConcatenatedStringBuilderForEachExpansion(IEnumerable<List<IList<IStringBuilder>>> expandedUnionList)
        {
            var resultList = new List<IStringBuilder>();

            foreach (var expandedUnionExpression in expandedUnionList)
            {
                IStringBuilder stringBuilder = StringBuilder.Empty;

                foreach (var singleStringBuilder in expandedUnionExpression)
                    stringBuilder = stringBuilder.ConcatWith(singleStringBuilder.FirstOrDefault());

                resultList.Add(stringBuilder);
            }

            return resultList;
        }

        private ConcatenationExpression AppendRange(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
                _elementaryExpressions.Add(expression);
    
            return this;
        }
        
        private static ExpansionStatus WasExpandedRepeatExpressionAddedToList(List<IExpression> repeatExpressions, ICollection<List<IExpression>> newExpandList, int concatElementPosition)
        {
            var repeatExpression = repeatExpressions[concatElementPosition];
            
            repeatExpression.RepeatRange.ToExpansionBounds(out var start, out var end);

            if (repeatExpression.ExpansionLength != null &&  repeatExpression.ExpansionLength < end)
                end = repeatExpression.ExpansionLength.Value + 1;
            
            if (start == 1 && end == 1)
                return ExpansionStatus.NotExpanded;

            for (var i = start; i <= end; i++)
            {
                var distanceConcatPositionToListEnd = repeatExpressions.Count - concatElementPosition - 1;
                var expandedExpressions = new List<IExpression>();
                
                expandedExpressions.AddRange(repeatExpressions.GetRange(0, concatElementPosition));
                expandedExpressions.AddRange(ConcatElementaryExpressionTimes(repeatExpression, i));
                expandedExpressions.AddRange(repeatExpressions.GetRange(concatElementPosition + 1,distanceConcatPositionToListEnd));
                
                if(!expandedExpressions.Any())
                    expandedExpressions.Add(new EmptyExpression());
                
                newExpandList.Add(expandedExpressions);
            }

            return ExpansionStatus.Expanded;
        }

        private static IEnumerable<IExpression> ConcatElementaryExpressionTimes(IExpression expression, int concatTimes)
        {
            var expandedExpression = new List<IExpression>();

            var expressionClone = expression.Clone();
            
            expressionClone.RepeatRange = new IntegerInterval(1);
            
            for (var i = 0; i < concatTimes; i++)
                expandedExpression.Add(expressionClone);
            
            return expandedExpression;
        }
        
        private IExpression GetInverseOfExpandedConcatRepresentation(List<IExpression> expandedConcatRepresentation)
        {
            var inverseList 
                = expandedConcatRepresentation
                    .Select((t, position) => GetInverseOfExpandedConcatForPosition(expandedConcatRepresentation, position))
                    .Cast<IExpression>()
                    .ToList();

            return new UnionExpression(inverseList, _random);
        }
        
        private ConcatenationExpression GetInverseOfExpandedConcatForPosition(List<IExpression> expandedConcatRepresentation, int i)
        {
            var predecessorRange = expandedConcatRepresentation.GetRange(0, i);
            var inverse = expandedConcatRepresentation[i].GetInverse();
            var successorRange = expandedConcatRepresentation.GetRange(i + 1, expandedConcatRepresentation.Count - i - 1);

            var concatExpression = new ConcatenationExpression(_random);
            concatExpression.AppendRange(predecessorRange)
                            .Append(inverse)
                            .AppendRange(successorRange);
            return concatExpression;
        }

        private static bool IsExpansionFactorValid(int baseExpansionLength, int expansionFactor, int maxExpansionLength) =>
            baseExpansionLength != int.MaxValue &&
            expansionFactor != int.MaxValue &&
            (double) baseExpansionLength / int.MaxValue * expansionFactor < 1 &&
            baseExpansionLength * expansionFactor < int.MaxValue - maxExpansionLength;
    }
}