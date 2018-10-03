using System;

namespace Regseed.Common.Token
{
    public abstract class BaseToken : IToken
    {
        protected BaseToken(long position)
        {
            Position = position;
        }
        
        protected BaseToken(long position, int length)
        {
            Position = position;
            Length = length;
        }

        public abstract TEnum GetType<TEnum>() where TEnum : struct;

        public virtual TValue GetValue<TValue>()
        {
            return default(TValue);
        }

        public long Position { get; }
        public virtual int Length { get; }
    }
    
    public abstract class BaseToken<TValueImpl> : BaseToken
    {
        private readonly TValueImpl _value;

        protected BaseToken(TValueImpl value, long position) : base(position)
        {
            _value = value;
        }

        protected BaseToken(TValueImpl value, long position, int length) : base(position, length)
        {
            _value = value;
        }

        public override TValue GetValue<TValue>()
        {
            if(typeof(TValue) != typeof(TValueImpl))
                throw new TypeAccessException("GetValue called with wrong generic type...");

            return (TValue)(object)_value;
        }
    }
}