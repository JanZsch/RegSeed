using Regseed.Common.Results;

namespace Regseed.Common.Ranges
{
    internal class IntegerInterval : BaseRange<int?>
    {
        public IntegerInterval() : base(null, null)
        {}

        public IntegerInterval(int? value) : base(value, value)
        {}
        
        public IResult TrySetValue(int? start, int? end)
        {
            if (start != null && end != null && start.Value > end.Value) 
                return new FailureResult();
            
            Start = start;
            End = end;
            
            return new SuccessResult();
        }

        public IntegerInterval Clone()
        {
            var clone = new IntegerInterval();
            clone.TrySetValue(Start, End);
            return clone;
        }
    }
}