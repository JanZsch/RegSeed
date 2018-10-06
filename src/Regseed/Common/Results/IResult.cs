namespace Regseed.Common.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
    }
    
    public interface IResult<TValue>
    {
        bool IsSuccess { get; }
        TValue Value { get; }
    }
}