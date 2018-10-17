using System.Collections.Generic;
using Regseed.Common.Random;
using Regseed.Factories;

namespace Regseed.Expressions
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
        
        protected override IStringBuilder ToSingleStringBuilder()
        {
            IStringBuilder builder = StringBuilder.Empty;

            foreach (var expression in _expressions)
                builder = builder.ConcatWith(expression.ToStringBuilder());

            return builder;
        }
    }
}