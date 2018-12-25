using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class SeedUnionExpression : UnionExpression
    {
        public SeedUnionExpression(List<IExpression> expressions, IRandomGenerator random) : base(expressions, random)
        {
        }

        public override void SetExpansionLength(int _ = 0)
        {
            foreach (var intersectExpression in _intersectExpressions)
            {
                intersectExpression.MaxExpansionRange.ToExpansionBounds(out var lower, out var upper);
                var rand = _random.GetNextInteger(lower, upper);
                intersectExpression.SetExpansionLength(rand);
            }
        }

        public override IList<IStringBuilder> Expand()
        {
            var indexList = Enumerable.Range(0, _intersectExpressions.Count).ToList();
            indexList.Shuffle(_random);

            var n = 0;

            while (n < indexList.Count)
            {
                var expandedExpression = ExpandIntersectionExpression(_intersectExpressions[indexList[n]]);

                if (expandedExpression.Any())
                    return expandedExpression;

                n++;
            }

            return new List<IStringBuilder>();
        }

        private IList<IStringBuilder> ExpandIntersectionExpression(IExpression intersectExpression)
        {
            intersectExpression.MaxExpansionRange.ToBounds(out var lower, out var upper);
            var expansionLengthRange = Enumerable.Range(lower, upper - lower).ToList();
            expansionLengthRange.Shuffle(_random);

            IList<IStringBuilder> stringBuilderList = new List<IStringBuilder>();

            var n = 0;
            while ( n < expansionLengthRange.Count)
            {
                intersectExpression.SetExpansionLength(expansionLengthRange[n]);
                stringBuilderList = intersectExpression.Expand();

                if (stringBuilderList.Any())
                    return stringBuilderList;

                n++;
            }

            return stringBuilderList;
        }
    }
}