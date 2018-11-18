using System;
using Regseed.Common.Ranges;
using Regseed.Resources;

namespace Regseed.Parser.RegexTokens
{
    internal class CharacterRangeToken : BaseToken<CharacterRange>
    {
        public CharacterRangeToken(CharacterRange value, long position, int length) : base(value, position, length)
        {
        }

        public override TEnum GetType<TEnum>()
        {
            if (typeof(TEnum) != typeof(RegexTokenType))
                throw new TypeAccessException(RegSeedErrorMessages.RegexTypeExpected);

            return (TEnum) (object) RegexTokenType.CharacterRange;
        }
    }
}