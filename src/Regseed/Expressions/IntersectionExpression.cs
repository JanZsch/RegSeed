using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Regseed.Common.Builder;
using Regseed.Common.Helper;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class IntersectionExpression : BaseExpression
    {
        private readonly List<IExpression> _concatExpressions;
        
        public IntersectionExpression(IEnumerable<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _concatExpressions = new List<IExpression>();
            
            foreach (var expression in expressions)
            {
                if (expression is IntersectionExpression intersectionExpression)
                    _concatExpressions.AddRange(intersectionExpression._concatExpressions);
                else
                    _concatExpressions.Add(expression);
            }
        }

        public override void SetExpansionLength(int expansionLength = 0)
        {
            ExpansionLength = expansionLength;
            
            foreach (var concatExpression in _concatExpressions)
                concatExpression.SetExpansionLength(expansionLength);
        }

        public override IList<IStringBuilder> Expand()
        {           
            var seedContent = new List<IList<IStringBuilder>>();
            var expandGuard = new object();

            var reducedList = RemoveTooLongExpressions(ExpansionLength, _concatExpressions);
            
            Parallel.ForEach(reducedList, concatExpression =>
            {
                var expansion = concatExpression.Expand();

                lock (expandGuard)
                    seedContent.Add(expansion);
            });

            var trimmedSeedContent = RemoveTooLongExpressions(ExpansionLength, seedContent);
            
            var seed = new List<List<IList<IStringBuilder>>> { trimmedSeedContent };
            
            var expandedStringBuilderList = ExpandHelper.ExpandListRepresentation(seed, null, ExpandHelper.WasExpandedStringBuilderListAddedToList);
            
            return MergeStringBuildersForEachUnion(expandedStringBuilderList);
        }

        public override IExpression GetInverse()
        {
            var concatInverses = new List<IExpression>();
            var addGuard = new object();

            Parallel.ForEach(_concatExpressions, expression =>
            {
                var inverse = expression.GetInverse();
            
                lock(addGuard)
                    concatInverses.Add(inverse);
            });

            return new UnionExpression(concatInverses, _random)
            {
                RepeatRange = RepeatRange
            };
        }

        public override IExpression Clone() =>
            new IntersectionExpression(_concatExpressions.Select(x => x.Clone()).ToList(), _random)
            {
                RepeatRange = RepeatRange?.Clone(),
                ExpansionLength = ExpansionLength
            };

        protected override IntegerInterval GetMaxExpansionInterval ()
        {          
            var minExpansion = 0;
            var maxExpansion = int.MaxValue;

            foreach (var concatExpression in _concatExpressions)
            {
                concatExpression.MaxExpansionRange.ToExpansionBounds(out var minExpansionLength, out var maxExpansionLength);

                if(minExpansionLength > maxExpansion || maxExpansionLength < minExpansion)
                    return new IntegerInterval(0);
                
                if (minExpansion < minExpansionLength)
                    minExpansion = minExpansionLength;
                
                if (maxExpansionLength < maxExpansion)
                    maxExpansion = maxExpansionLength;
                
                if(minExpansion == 0 && maxExpansion == 0)
                    return new IntegerInterval(0);
            }

            RepeatRange.ToExpansionBounds(out var lowerFactor, out var upperFactor);
            var minExpansionInterval = new IntegerInterval();
            minExpansionInterval.TrySetValue(minExpansion*lowerFactor, maxExpansion*upperFactor);
            
            return minExpansionInterval;
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_concatExpressions == null || !_concatExpressions.Any())
                return StringBuilder.Empty;

            var intersection = _concatExpressions[0].ToStringBuilder();

            for (var i = 1; i < _concatExpressions.Count; i++)
                intersection = intersection.IntersectWith(_concatExpressions[i].ToStringBuilder());

            return intersection;
        }

        internal IList<IExpression> ToConcatExpressionList() => _concatExpressions.ToList();
        
        private static IEnumerable<IExpression> RemoveTooLongExpressions(int? maxExpansionLength, IEnumerable<IExpression> expressions)
        {
            if (maxExpansionLength == null)
                return expressions;
            
            return expressions.Where(x =>
            {
                x.MaxExpansionRange.ToLowerExpansionBound(out var lowerBound);
                return lowerBound <= maxExpansionLength;
            });
        }
        
        private static List<IList<IStringBuilder>> RemoveTooLongExpressions(int? expansionLength, List<IList<IStringBuilder>> seedContent)
        {
            if (expansionLength == null)
                return seedContent;

            var result = new List<IList<IStringBuilder>>();
            
            foreach (var intersectListRepresentation in seedContent)
            {
                var intersectCandidate = intersectListRepresentation.Where(y => y.GeneratedStringLength() <= expansionLength).ToList();
                
                if(intersectCandidate.Any())
                    result.Add(intersectCandidate);                
            }
            
            return result;
        }
        
        private static IList<IStringBuilder> MergeStringBuildersForEachUnion(IEnumerable<List<IList<IStringBuilder>>> intersectedStringBuilderUnion)
        {
            var result = new List<IStringBuilder>();

            var addGuard = new object();
            
            Parallel.ForEach(intersectedStringBuilderUnion, intersectStringBuilders =>
            {
                MergeAndAddStringBuilderForSingleUnionRepresentation(intersectStringBuilders, result, addGuard);
            });

            return result;
        }
        
        private static void MergeAndAddStringBuilderForSingleUnionRepresentation(IList<IList<IStringBuilder>> intersectStringBuilders, List<IStringBuilder> result, object addGuard)
        {
            if(!StringBuilderHelper.IsStringBuildMergingRequired(intersectStringBuilders))
                return;
            
            var intersection = intersectStringBuilders.LastOrDefault()?.FirstOrDefault();

            if (intersection == null)
                return;

            intersectStringBuilders.RemoveAt(intersectStringBuilders.Count-1);
            
            foreach (var intersectStringBuilderListRepresentation in intersectStringBuilders)
            {
                var stringBuilder = intersectStringBuilderListRepresentation.FirstOrDefault();
                
                if(stringBuilder == null)
                    return;

                intersection = intersection.IntersectWith(stringBuilder);
                
                if(intersection.GeneratedStringLength() == 0)
                    return;
            }
            
            lock(addGuard)
                result.Add(intersection);
        }
    }
}