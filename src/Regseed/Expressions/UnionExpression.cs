using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class UnionExpression : BaseExpression
    {
        protected readonly List<IExpression> _intersectExpressions;
        
        public UnionExpression(IEnumerable<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _intersectExpressions = new List<IExpression>();
            
            foreach (var expression in expressions ?? new List<IExpression>())
            {
                if (expression is UnionExpression unionExpression)
                    _intersectExpressions.AddRange(unionExpression._intersectExpressions);
                else
                    _intersectExpressions.Add(expression);
            }
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
            var expandGuard = new object();
            
            Parallel.ForEach(_intersectExpressions, intersectExpression =>
            {
                var expansion = intersectExpression.Expand();

                lock (expandGuard)
                    expandedList.AddRange(expansion);
            });

            return expandedList;
        }

        public override IExpression GetInverse(int inverseLength) =>
            new IntersectionExpression(_intersectExpressions.Select(x => x.GetInverse(inverseLength)).ToList(), _random)
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

            RepeatRange.ToExpansionBounds(out var lowerFactor, out var upperFactor);
            var maxExpansionInterval = new IntegerInterval();
            maxExpansionInterval.TrySetValue(minExpansion*lowerFactor, maxExpansion*upperFactor);
            
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