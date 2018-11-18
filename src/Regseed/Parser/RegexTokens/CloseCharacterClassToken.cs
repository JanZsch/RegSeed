namespace Regseed.Parser.RegexTokens
{
    internal class CloseCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public CloseCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.CloseCharacterClass;
        }
    }
}