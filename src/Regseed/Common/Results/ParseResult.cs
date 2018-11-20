using Regseed.Resources;

namespace Regseed.Common.Results
{
    internal class ParseResult : BaseResult, IParseResult
    {
        public ParseResult(bool isSuccess, RegSeedErrorType errorType) : base(isSuccess)
        {
            ErrorType = errorType;
        }

        public long Position { get; set; }
        public RegSeedErrorType ErrorType { get; set; }
    }
    
    internal class SuccessParseResult : ParseResult
    {
        public SuccessParseResult(long position = 0, RegSeedErrorType errorType = RegSeedErrorType.None) : base(true, errorType)
        {
            Position = position;
        }
    }
    
    internal class FailureParseResult : ParseResult
    {
        public FailureParseResult(long position, RegSeedErrorType errorType = RegSeedErrorType.None) : base(false, errorType)
        {
            Position = position;
        }
    }
    
    internal class ParseResult<TValue> : BaseResult<TValue>, IParseResult<TValue>
    {
        public ParseResult(bool isSuccess, TValue value, RegSeedErrorType errorType = RegSeedErrorType.None) : base(isSuccess, value)
        {
            ErrorType = errorType;
        }

        public long Position { get; set; }
        public RegSeedErrorType ErrorType { get; set; }
    }
    
    internal class SuccessParseResult<TValue> : ParseResult<TValue>
    {
        public SuccessParseResult(long position, TValue value, RegSeedErrorType errorType = RegSeedErrorType.None) : base(true, value, errorType)
        {
            Position = position;
        }
    }
    
    internal class FailureParseResult<TValue> : ParseResult<TValue>
    {
        public FailureParseResult(long position, RegSeedErrorType errorType = RegSeedErrorType.None) : base(false, default (TValue), errorType)
        {
            Position = position;
        }
    }
}