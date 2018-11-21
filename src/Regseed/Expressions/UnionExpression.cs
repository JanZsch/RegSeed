using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;
using Regseed.Common.Random;

namespace Regseed.Expressions
{
    internal class UnionExpression : BaseExpression
    {
        private readonly List<IExpression> _intersectExpressions;
        
        public UnionExpression(List<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _intersectExpressions = expressions;
        }

        public override IList<IStringBuilder> Expand()
        {
            var expandedList = new List<IStringBuilder>();

            foreach (var intersectExpression in _intersectExpressions)
                expandedList.AddRange(intersectExpression.Expand());

            return expandedList;
        }

        public override IExpression GetInverse()
        {
            return new IntersectionExpression(_intersectExpressions.Select(x => x.GetInverse()).ToList(), _random)
            {
                RepeatRange = RepeatRange
            };
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_intersectExpressions.Count == 0)
                return StringBuilder.Empty;

            return _intersectExpressions.Count == 1
                ? _intersectExpressions[0].ToStringBuilder()
                : _intersectExpressions[_random.GetNextInteger(0, _intersectExpressions.Count)].ToStringBuilder();
        }

        internal IList<IExpression> ToIntersectionExpressionList() => 
            _intersectExpressions.ToList();
    }
}