namespace Regseed.Parser.RegexTokens
{
    internal class ClosePrecedenceToken : RegexSingleCharacterBaseToken
    {
        public ClosePrecedenceToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.ClosePrecedence;
        }
    }
}