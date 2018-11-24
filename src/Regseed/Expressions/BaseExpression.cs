using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal abstract class BaseExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }
        
        protected readonly IRandomGenerator _random;

        protected BaseExpression(IRandomGenerator random)
        {
            _random = random;
        }

        public virtual IStringBuilder ToStringBuilder()
        {
            RepeatRange.ToBounds(out var lowerBound, out var upperBound);
            var repeatedTimes = _random.GetNextInteger(lowerBound, upperBound);
            var singleStringBuilder = ToSingleStringBuilder();
            
            return StringBuilder.Empty.ConcatWith(singleStringBuilder, repeatedTimes);
        }

        public abstract IList<IStringBuilder> Expand();
        
        public abstract IExpression GetInverse();
        public abstract IExpression Clone();

        protected abstract IStringBuilder ToSingleStringBuilder();
    }
}