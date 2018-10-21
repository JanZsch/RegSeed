using System.Collections.Generic;
using System.Linq;
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

        public override IExpression GetComplement()
        {
            var inverseConcatExpression = _concatExpressions.Select(x => x.GetComplement()).ToList();
            
            return new MergeExpression(inverseConcatExpression, _random)
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