using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Random;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal class UnionExpression : BaseExpression
    {
        protected readonly List<IExpression> _intersectExpressions;
        
        public UnionExpression(List<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _intersectExpressions = expressions;
        }

        public override IExpression GetComplement()
        {
            return new UnionExpression(_intersectExpressions.Select(x => x.GetComplement()).ToList(), _random)
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
    }
}