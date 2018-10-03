namespace Regseed.Common.Results
{
    public class ParseResult : BaseResult, IParseResult
    {
        public ParseResult(bool isSuccess, string message) : base(isSuccess, message)
        {
        }

        public long Position { get; set; }
    }
    
    public class SuccessParseResult : ParseResult
    {
        public SuccessParseResult(long position = 0, string message = null) : base(true, message)
        {
            Position = position;
        }
    }
    
    public class FailureParseResult : ParseResult
    {
        public FailureParseResult(long position, string message = null) : base(false, message)
        {
            Position = position;
        }
    }
    
    public class ParseResult<TValue> : BaseResult<TValue>, IParseResult<TValue>
    {
        public ParseResult(bool isSuccess, TValue value, string message) : base(isSuccess, value, message)
        {
        }

        public long Position { get; set; }
    }
    
    public class SuccessParseResult<TValue> : ParseResult<TValue>
    {
        public SuccessParseResult(long position, TValue value, string message = null) : base(true, value, message)
        {
            Position = position;
        }
    }
    
    public class FailureParseResult<TValue> : ParseResult<TValue>
    {
        public FailureParseResult(long position, string message = null) : base(false, default (TValue), message)
        {
            Position = position;
        }
    }
}