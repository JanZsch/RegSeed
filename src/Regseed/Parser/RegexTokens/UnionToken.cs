namespace Regseed.Parser.RegexTokens
{
    internal class UnionToken : RegexSingleCharacterBaseToken
    {
        public UnionToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Union;
        }
    }
}