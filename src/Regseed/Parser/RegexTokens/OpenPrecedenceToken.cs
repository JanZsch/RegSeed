namespace Regseed.Parser.RegexTokens
{
    internal class OpenPrecedenceToken : RegexSingleCharacterBaseToken
    {
        public OpenPrecedenceToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenPrecedence;
        }
    }
}