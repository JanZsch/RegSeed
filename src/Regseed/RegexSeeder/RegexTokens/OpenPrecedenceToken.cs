namespace Regseed.RegexSeeder.RegexTokens
{
    public class OpenPrecedenceToken : RegexSingleCharacterBaseToken
    {
        public OpenPrecedenceToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenPrecedence;
        }
    }
}