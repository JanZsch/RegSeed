using Regseed.Resources;

namespace Regseed.Common.Results
{
    public interface IParseResult : IResult
    {
        long Position { get; }
        RegSeedErrorType ErrorType { get; }
    }
    
    internal interface IParseResult<TValue> : IParseResult
    {
        TValue Value { get; }
    }
}