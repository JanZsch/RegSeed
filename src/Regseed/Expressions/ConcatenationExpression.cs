using System.Collections.Generic;
using System.Linq;
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
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, WasExpandedRepeatExpressionAddedToList);

            var intersectInverseList = expandedRepeatRepresentation.Select(GetInverseOfExpandedConcatRepresentation).ToList();
            
            return new IntersectionExpression(intersectInverseList, _random);
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

        public override IList<IStringBuilder> Expand()
        {
            var seed = new List<List<IExpression>> {_elementaryExpressions};
            
            var expandedRepeatRepresentation = ExpandHelper.ExpandListRepresentation(seed, WasExpandedRepeatExpressionAddedToList);

            var convertedExpressions = new List<List<IList<IStringBuilder>>>();

            foreach (var expressionList in expandedRepeatRepresentation)
            {
                var expandedExpression = expressionList.Select(x => x.Expand()).ToList();
                convertedExpressions.Add(expandedExpression);
            }

            var expandedUnionList = ExpandHelper.ExpandListRepresentation(convertedExpressions, ExpandHelper.WasExpandedStringBuilderListAddedToList);

            return CreateConcatenatedStringBuilderForEachExpansion(expandedUnionList);
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            IStringBuilder builder = StringBuilder.Empty;

            foreach (var expression in _elementaryExpressions)
                builder = builder.ConcatWith(expression.ToStringBuilder());

            return builder;
        }

        internal IList<IExpression> ToConcatExpressionList() => _elementaryExpressions.ToList();
        
        private static IList<IStringBuilder> CreateConcatenatedStringBuilderForEachExpansion(List<List<IList<IStringBuilder>>> expandedUnionList)
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
        
        private static bool WasExpandedRepeatExpressionAddedToList(List<IExpression> repeatExpressions, List<List<IExpression>> newExpandList, int concatElementPosition)
        {
            var repeatExpression = repeatExpressions[concatElementPosition];
            repeatExpression.RepeatRange.ToBounds(out var start, out var end);

            if (start == 1 && end == start +1)
                return false;

            for (var i = start; i < end; i++)
            {
                var distanceConcatPositionToListEnd = repeatExpressions.Count - concatElementPosition - 1;
                var expandedExpressions = new List<IExpression>();
                expandedExpressions.AddRange(repeatExpressions.GetRange(0, concatElementPosition));
                expandedExpressions.AddRange(ConcatElementaryExpression(repeatExpression, i));
                expandedExpressions.AddRange(repeatExpressions.GetRange(concatElementPosition + 1,distanceConcatPositionToListEnd));
                newExpandList.Add(expandedExpressions);
            }

            return true;
        }

        private static IEnumerable<IExpression> ConcatElementaryExpression(IExpression expression, int concatTimes)
        {
            var expandedExpression = new List<IExpression>();

            expression.RepeatRange = new IntegerInterval(1);
            
            for (var i = 0; i < concatTimes; i++)
                expandedExpression.Add(expression);
            
            return expandedExpression;
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
    }
}