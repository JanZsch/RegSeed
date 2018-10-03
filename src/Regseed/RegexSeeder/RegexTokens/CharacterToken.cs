using System;
using Regseed.Common.Resources;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens
{
    public class CharacterToken : BaseToken<string>
    {
        public CharacterToken(string value, long position, int length) : base(value, position, length)
        {
        }

        public override TEnum GetType<TEnum>()
        {
            if (typeof(TEnum) != typeof(RegexTokenType))
                throw new TypeAccessException(ParserMessages.RegexTypeExpected);

            return (TEnum)(object) RegexTokenType.Character;
        }
    }
}