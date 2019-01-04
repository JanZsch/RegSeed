using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal abstract class BaseExpression : IExpression
    {
        private IntegerInterval _maxExpansionInterval;

        public int? ExpansionLength { get; protected set; }

        public IntegerInterval MaxExpansionRange => _maxExpansionInterval ?? (_maxExpansionInterval = GetMaxExpansionInterval());

        public IntegerInterval RepeatRange { get; set; }
        
        protected readonly IRandomGenerator _random;
        
        protected BaseExpression(IRandomGenerator random)
        {
            _random = random;
        }
        
        public virtual IStringBuilder ToStringBuilder()
        {
            RepeatRange.ToExpansionBounds(out var lowerBound, out var upperBound);
            var repeatedTimes = _random.GetNextInteger(lowerBound, upperBound);
            var singleStringBuilder = ToSingleStringBuilder();
            
            return StringBuilder.Empty.ConcatWith(singleStringBuilder, repeatedTimes);
        }

        public abstract void SetExpansionLength(int expansionLength = 0);
        public abstract IList<IStringBuilder> Expand();
        public abstract IExpression GetInverse(int inverseLength);
        public abstract IExpression Clone();

        protected abstract IntegerInterval GetMaxExpansionInterval();
        protected abstract IStringBuilder ToSingleStringBuilder();
    }
}