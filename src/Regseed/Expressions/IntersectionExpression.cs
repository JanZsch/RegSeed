using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Helper;
using Regseed.Common.Random;
using Regseed.Factories;

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

            var seed = new List<List<IList<IStringBuilder>>>{seedContent};
            
            var expandedStringBuilderList = ExpandHelper.ExpandListRepresentation(seed, ExpandHelper.WasExpandedStringBuilderListAddedToList);
            
            return MergeStringBuildersForEachUnion(expandedStringBuilderList);
        }

        internal IList<IExpression> ToConcatExpressionList() => _concatExpressions.ToList();
        
        private static IList<IStringBuilder> MergeStringBuildersForEachUnion(List<List<IList<IStringBuilder>>> intersectedStringBuilderUnion)
        {
            var result = new List<IStringBuilder>();

            foreach (var intersectStringBuilders in intersectedStringBuilderUnion)
            {
                var intersection = intersectStringBuilders.FirstOrDefault()?.FirstOrDefault();

                if (intersection == null)
                    continue;

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
                
                if(doAllStringBuildersCreateStringsOfSameLength && intersection.GeneratedStringLength() != 0)
                    result.Add(intersection);                
            }

            return result;
        }

        public override IExpression GetInverse()
        {
            return new UnionExpression(_concatExpressions.Select(x => x.GetInverse()).ToList(), _random)
            {
                RepeatRange = RepeatRange
            };
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_concatExpressions == null || !_concatExpressions.Any())
                return StringBuilder.Empty;

            IStringBuilder builder = null;

            foreach (var concatExpression in _concatExpressions)
            {
                if (builder == null)
                {
                    builder = concatExpression.ToStringBuilder();
                    continue;
                }

                builder = builder.IntersectWith(concatExpression.ToStringBuilder());
            }

            return builder;
        }
    }
}