namespace Regseed.Common.Ranges
{
    internal abstract class BaseRange<TType>
    {
        public TType Start { get; protected set; }
        public TType End { get; protected set; }
        
        protected BaseRange(TType start, TType end)
        {
            Start = start;
            End = end;
        }
    }
}