namespace Regseed.Parser.RegexTokens
{
    public class OpenNegatedCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public OpenNegatedCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenNegateCharacterClass;
        }
    }
}