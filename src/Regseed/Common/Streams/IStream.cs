namespace Regseed.Common.Streams
{
    public interface IStream<TValue>
    {
        long Count { get; }
        long CurrentPosition { get; }
        
        TValue Pop();
        TValue LookAhead(long pos);
        IStream<TValue> Append(TValue value);
        bool IsEmpty();
        void Flush();
    }
}