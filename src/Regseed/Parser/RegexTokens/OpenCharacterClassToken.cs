namespace Regseed.Parser.RegexTokens
{
    internal class OpenCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public OpenCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenCharacterClass;
        }
    }
}