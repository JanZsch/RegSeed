namespace Regseed.RegexSeeder.RegexTokens
{
    public class UnionToken : RegexSingleCharacterBaseToken
    {
        public UnionToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Union;
        }
    }
}