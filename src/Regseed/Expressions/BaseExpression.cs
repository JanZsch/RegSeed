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

        public virtual string ToRegexString()
        {
            var repeatRange = RepeatRange ?? new IntegerInterval(1);
            var lowerBound = repeatRange.Start == null || repeatRange.Start < 0 ? 0 : repeatRange.Start.Value;
            var upperBound = repeatRange.End ?? int.MaxValue - 1;
            upperBound = upperBound + 1 < 0 ? 0 : upperBound + 1;

            var repetitions = _random.GetNextInteger(lowerBound, upperBound);

            var result = string.Empty;
            for (var i = 0; i < repetitions; i++)
                result = $"{result}{ToSingleRegexString()}";

            return result;
        }

        public abstract IExpression GetComplement();

        protected abstract string ToSingleRegexString();
    }
}