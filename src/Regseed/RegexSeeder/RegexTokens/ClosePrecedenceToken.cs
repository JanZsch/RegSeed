namespace Regseed.RegexSeeder.RegexTokens
{
    public class ClosePrecedenceToken : RegexSingleCharacterBaseToken
    {
        public ClosePrecedenceToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.ClosePrecedence;
        }
    }
}