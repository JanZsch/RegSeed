using Regseed.Resources;

namespace Regseed.Common.Results
{
    public class ParseResult : BaseResult, IParseResult
    {
        public ParseResult(bool isSuccess, RegSeedErrorType errorType) : base(isSuccess)
        {
            ErrorType = errorType;
        }

        public long Position { get; set; }
        public RegSeedErrorType ErrorType { get; set; }
    }
    
    public class SuccessParseResult : ParseResult
    {
        public SuccessParseResult(long position = 0, RegSeedErrorType errorType = RegSeedErrorType.Unknown) : base(true, errorType)
        {
            Position = position;
        }
    }
    
    public class FailureParseResult : ParseResult
    {
        public FailureParseResult(long position, RegSeedErrorType errorType = RegSeedErrorType.Unknown) : base(false, errorType)
        {
            Position = position;
        }
    }
    
    public class ParseResult<TValue> : BaseResult<TValue>, IParseResult<TValue>
    {
        public ParseResult(bool isSuccess, TValue value, RegSeedErrorType errorType = RegSeedErrorType.Unknown) : base(isSuccess, value)
        {
            ErrorType = errorType;
        }

        public long Position { get; set; }
        public RegSeedErrorType ErrorType { get; set; }
    }
    
    public class SuccessParseResult<TValue> : ParseResult<TValue>
    {
        public SuccessParseResult(long position, TValue value, RegSeedErrorType errorType = RegSeedErrorType.Unknown) : base(true, value, errorType)
        {
            Position = position;
        }
    }
    
    public class FailureParseResult<TValue> : ParseResult<TValue>
    {
        public FailureParseResult(long position, RegSeedErrorType errorType = RegSeedErrorType.Unknown) : base(false, default (TValue), errorType)
        {
            Position = position;
        }
    }
}