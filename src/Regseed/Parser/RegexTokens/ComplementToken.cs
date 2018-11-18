namespace Regseed.Parser.RegexTokens
{
    internal class ComplementToken : RegexSingleCharacterBaseToken
    {
        public ComplementToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Complement;
        }
    }
}