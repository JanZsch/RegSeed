namespace Regseed.Common.Results
{
    public class BaseResult : IResult
    {
        public BaseResult(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; set; }
    }

    public class SuccessResult : BaseResult
    {
        public SuccessResult(string message = null) : base(true, message)
        {
        }
    }
    
    public class FailureResult : BaseResult
    {
        public FailureResult(string message = null) : base(false, message)
        {
        }
    }
    
    public class BaseResult<TValue> : IResult<TValue>
    {
        public bool IsSuccess { get; }
        public TValue Value { get; }
        public string Message { get; set; }
        
        public BaseResult(bool isSuccess, TValue value, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Value = value;
        }
    }

    public class SuccessResult<TValue> : BaseResult<TValue>
    {
        public SuccessResult(TValue value, string message = null) : base(true, value, message)
        {
        }
    }
    
    public class FailureResult<TValue> : BaseResult<TValue>
    {
        public FailureResult(string message = null) : base(false, default(TValue), message)
        {
        }
    }
}