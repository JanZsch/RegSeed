using System;
using Regseed.Common.Resources;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens
{
    public abstract class RegexSingleCharacterBaseToken : BaseToken
    {
        protected RegexTokenType _tokenType;
        
        protected RegexSingleCharacterBaseToken() : base(0, 1)
        {
        }
        
        protected RegexSingleCharacterBaseToken(long position) : base(position, 1)
        {
        }

        public override TEnum GetType<TEnum>()
        {
            if (typeof(TEnum) != typeof(RegexTokenType))
                throw new TypeAccessException(ParserMessages.RegexTypeExpected);

            return (TEnum)(object) _tokenType;
        }
    }
}