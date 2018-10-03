namespace Regseed.Common.Results
{
    public interface IParseResult : IResult
    {
        long Position { get; set; }
    }
    
    public interface IParseResult<TValue> : IParseResult
    {
        TValue Value { get; }
    }
}