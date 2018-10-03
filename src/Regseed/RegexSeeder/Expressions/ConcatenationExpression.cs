using System.Collections.Generic;
using System.Text;
using Regseed.Common.Random;

namespace Regseed.RegexSeeder.Expressions
{
    internal class ConcatenationExpression : BaseExpression
    {
        private readonly List<IExpression> _expressions;
        
        public ConcatenationExpression(IRandomGenerator random) : base(random)
        {
            _expressions = new List<IExpression>();
        }

        public ConcatenationExpression Append(IExpression expression)
        {
            _expressions.Add(expression);
            return this;
        }

        protected override string ToSingleRegexString()
        {
            var builder = new StringBuilder();

            foreach (var expression in _expressions)
                builder.Append(expression.ToRegexString());

            return builder.ToString();
        }

        public override IExpression GetComplement()
        {
            var complementExpression = new ConcatenationExpression(_random)
            {
                RepeatRange = RepeatRange
            };

            foreach (var expression in _expressions)
                complementExpression.Append(expression.GetComplement());

            return complementExpression;
        }
    }
}