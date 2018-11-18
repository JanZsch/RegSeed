namespace Regseed.Parser.RegexTokens
{
    internal class OpenNegatedCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public OpenNegatedCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenNegateCharacterClass;
        }
    }
}