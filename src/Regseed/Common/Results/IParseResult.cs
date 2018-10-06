using Regseed.Resources;

namespace Regseed.Common.Results
{
    public interface IParseResult : IResult
    {
        long Position { get; set; }
        RegSeedErrorType ErrorType { get; set; }
    }
    
    public interface IParseResult<TValue> : IParseResult
    {
        TValue Value { get; }
    }
}