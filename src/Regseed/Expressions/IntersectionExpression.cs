using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;
using Regseed.Common.Helper;
using Regseed.Common.Random;

namespace Regseed.Expressions
{
    internal class IntersectionExpression : BaseExpression
    {
        private readonly IList<IExpression> _concatExpressions;
        
        public IntersectionExpression(IList<IExpression> concatExpressions, IRandomGenerator random) : base(random)
        {
            _concatExpressions = concatExpressions;
        }

        public override IList<IStringBuilder> Expand()
        {
            var seedContent = _concatExpressions.Select(concatExpression => concatExpression.Expand()).ToList();

            var seed = new List<List<IList<IStringBuilder>>> { seedContent };
            
            var expandedStringBuilderList = ExpandHelper.ExpandListRepresentation(seed, ExpandHelper.WasExpandedStringBuilderListAddedToList);
            
            return MergeStringBuildersForEachUnion(expandedStringBuilderList);
        }

        public override IExpression GetInverse()
        {
            return new UnionExpression(_concatExpressions.Select(x => x.GetInverse()).ToList(), _random)
            {
                RepeatRange = RepeatRange
            };
        }

        public override IExpression Clone() =>
            new IntersectionExpression(_concatExpressions.Select(x => x.Clone()).ToList(), _random)
            {
                RepeatRange = RepeatRange?.Clone()
            };

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_concatExpressions == null || !_concatExpressions.Any())
                return StringBuilder.Empty;

            return _concatExpressions.Aggregate<IExpression, IStringBuilder>(null, (intersectionResult, concatExpression) => IntersectStringBuilderWith(concatExpression.ToStringBuilder(), intersectionResult));
        }

        internal IList<IExpression> ToConcatExpressionList() => _concatExpressions.ToList();
        
        private static IList<IStringBuilder> MergeStringBuildersForEachUnion(IEnumerable<List<IList<IStringBuilder>>> intersectedStringBuilderUnion)
        {
            var result = new List<IStringBuilder>();

            foreach (var intersectStringBuilders in intersectedStringBuilderUnion)
                MergeAndAddStringBuilderForSingleUnionRepresentation(intersectStringBuilders, result);

            return result;
        }
        
        private static void MergeAndAddStringBuilderForSingleUnionRepresentation(List<IList<IStringBuilder>> intersectStringBuilders, List<IStringBuilder> result)
        {
            var intersection = intersectStringBuilders.FirstOrDefault()?.FirstOrDefault();

            if (intersection == null)
                return;

            var resultStringLength = intersection.GeneratedStringLength();
            var doAllStringBuildersCreateStringsOfSameLength = true;

            foreach (var stringBuilder in intersectStringBuilders)
            {
                var currentStringBuilder = stringBuilder.FirstOrDefault();
                if (currentStringBuilder?.GeneratedStringLength() != resultStringLength)
                {
                    doAllStringBuildersCreateStringsOfSameLength = false;
                    break;
                }

                intersection = intersection.IntersectWith(currentStringBuilder);
            }

            if (doAllStringBuildersCreateStringsOfSameLength && intersection.GeneratedStringLength() != 0)
                result.Add(intersection);
        }
        
        private static IStringBuilder IntersectStringBuilderWith(IStringBuilder toIntersectStringBuilder, IStringBuilder intersectionResult)
        {
            if (intersectionResult == null)
            {
                intersectionResult = toIntersectStringBuilder;
                return intersectionResult;
            }

            intersectionResult = intersectionResult.IntersectWith(toIntersectStringBuilder);
            return intersectionResult;
        }
    }
}