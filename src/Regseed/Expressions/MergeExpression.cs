using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Random;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal class MergeExpression : BaseExpression
    {
        protected readonly IList<IExpression> _concatExpressions;
        
        public MergeExpression(IList<IExpression> concatExpressions, IRandomGenerator random) : base(random)
        {
            _concatExpressions = concatExpressions;
        }

        public override IExpression GetInverse()
        {
            var inverseConcatExpression = _concatExpressions.Select(x => x.GetInverse()).ToList();
            
            return new IntersectionExpression(inverseConcatExpression, _random)
            {
                RepeatRange = RepeatRange
            };
        }

        protected override IStringBuilder ToSingleStringBuilder()
        {
            if (_concatExpressions == null || !_concatExpressions.Any())
                return StringBuilder.Empty;

            var firstConcatExpression = _concatExpressions[0].ToStringBuilder();
            var builder = _concatExpressions.Aggregate(firstConcatExpression, (current, concatExpression) => current.MergeWith(concatExpression.ToStringBuilder()));

            return builder;
        }
    }
}