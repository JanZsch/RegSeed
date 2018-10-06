using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Random;

namespace Regseed.Expressions
{
    internal class UnionExpression : BaseExpression
    {
        protected readonly List<IExpression> _expressions;
        
        public UnionExpression(List<IExpression> expressions, IRandomGenerator random) : base(random)
        {
            _expressions = expressions;
        }

        public override IExpression GetComplement()
        {
            return new UnionExpression(_expressions.Select(x => x.GetComplement()).ToList(), _random)
            {
                RepeatRange = RepeatRange
            };
        }

        protected override string ToSingleRegexString()
        {
            if (_expressions.Count == 0)
                return string.Empty;

            return _expressions.Count == 1
                ? _expressions[0].ToRegexString()
                : _expressions[_random.GetNextInteger(0, _expressions.Count)].ToRegexString();
        }
    }
}