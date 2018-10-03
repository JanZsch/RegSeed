namespace Regseed.Common.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; set; }
    }
    
    public interface IResult<TValue>
    {
        bool IsSuccess { get; }
        string Message { get; set; }
        TValue Value { get; }
    }
}