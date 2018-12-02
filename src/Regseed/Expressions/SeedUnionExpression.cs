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

        public override void SetExpansionLength(int expansionLength)
        {
            foreach (var intersectExpression in _intersectExpressions)
            {
                MaxExpansionInterval.ToBounds(out var lower, out var upper);
                var rand = _random.GetNextInteger(lower, upper);
                intersectExpression.SetExpansionLength(upper);
            }
        }

        public override IList<IStringBuilder> Expand()
        {
            var indexList = Enumerable.Range(0, _intersectExpressions.Count).ToList();
            indexList.Shuffle(_random);

            var n = 0;

            while (n < indexList.Count)
            {
                var expandedExpression = _intersectExpressions[indexList[n]].Expand();

                if (expandedExpression.Any())
                    return expandedExpression;

                n++;
            }

            return new List<IStringBuilder>();
        }
    }
}