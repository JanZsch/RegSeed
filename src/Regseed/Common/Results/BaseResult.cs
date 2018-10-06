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
        public SuccessResult() : base(true)
        {
        }
    }

    public class FailureResult : BaseResult
    {
        public FailureResult() : base(false)
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
        public FailureResult() : base(false, default(TValue))
        {
        }
    }
}