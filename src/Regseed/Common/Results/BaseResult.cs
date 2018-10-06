namespace Regseed.Common.Results
{
    public class BaseResult : IResult
    {
        public BaseResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }
    }

    public class SuccessResult : BaseResult
    {
        public SuccessResult(string message = null) : base(true)
        {
        }
    }

    public class FailureResult : BaseResult
    {
        public FailureResult(string message = null) : base(false)
        {
        }
    }

    public class BaseResult<TValue> : IResult<TValue>
    {
        public BaseResult(bool isSuccess, TValue value)
        {
            IsSuccess = isSuccess;
            Value = value;
        }

        public bool IsSuccess { get; }
        public TValue Value { get; }
    }

    public class SuccessResult<TValue> : BaseResult<TValue>
    {
        public SuccessResult(TValue value, string message = null) : base(true, value)
        {
        }
    }

    public class FailureResult<TValue> : BaseResult<TValue>
    {
        public FailureResult(string message = null) : base(false, default(TValue))
        {
        }
    }
}