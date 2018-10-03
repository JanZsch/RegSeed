using System;
using Regseed.Common.Resources;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens
{
    public class EndOfStreamToken : IToken
    {
        public EndOfStreamToken(long pos)
        {
            Position = pos;
        }

        public TEnum GetType<TEnum>() where TEnum : struct
        {
            if (typeof(TEnum) != typeof(RegexTokenType))
                throw new TypeAccessException(ParserMessages.RegexTypeExpected);

            return (TEnum)(object) RegexTokenType.EndOfStream;
        }

        public TValue GetValue<TValue>()
        {
            return default(TValue);
        }

        public long Position { get; }
        public int Length  => 0;
    }
}