using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class UnionExpression : BaseExpression
    {
        protected readonly List<IExpression> _intersectExpressions;
        
        public UnionExpression(List<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _intersectExpressions = expressions;
        }

        public override void SetExpansionLength(int expansionLength = 0)
        {           
            foreach (var intersectExpression in _intersectExpressions)
                intersectExpression.SetExpansionLength(expansionLength);
            
            ExpansionLength = expansionLength;
        }

        public override IList<IStringBuilder> Expand()
        {
            var expandedList = new List<IStringBuilder>();

            foreach (var intersectExpression in _intersectExpressions)
                expandedList.AddRange(intersectExpression.Expand());                

            return expandedList;
        }

        public override IExpression GetInverse() =>
            new IntersectionExpression(_intersectExpressions.Select(x => x.GetInverse()).ToList(), _random)
            {
                RepeatRange = RepeatRange
            };

        public override IExpression Clone() =>
            new UnionExpression(_intersectExpressions.Select(x => x.Clone()).ToList(), _random)
            {
                RepeatRange = RepeatRange?.Clone(),
                ExpansionLength = ExpansionLength
            };

        protected override IntegerInterval GetMaxExpansionInterval()
        {
            var maxExpansion = 0;
            var minExpansion = int.MaxValue;

            foreach (var intersectExpression in _intersectExpressions)
            {
                intersectExpression.MaxExpansionRange.ToExpansionBounds(out var lowerBound, out var upperBound);

                if (upperBound > maxExpansion)
                    maxExpansion = upperBound;
                
                if (lowerBound < minExpansion)
                    minExpansion = lowerBound;

                if (minExpansion == 0 && maxExpansion == int.MaxValue)
                    return IntegerInterval.MaxInterval;
            }

            var maxExpansionInterval = new IntegerInterval();
            maxExpansionInterval.TrySetValue(minExpansion, maxExpansion);
            
            return maxExpansionInterval;
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_intersectExpressions.Count == 0)
                return StringBuilder.Empty;

            return _intersectExpressions.Count == 1
                ? _intersectExpressions[0].ToStringBuilder()
                : _intersectExpressions[_random.GetNextInteger(0, _intersectExpressions.Count - 1)].ToStringBuilder();
        }

        internal IList<IExpression> ToIntersectionExpressionList() => 
            _intersectExpressions.ToList();
    }
}