namespace Regseed.RegexSeeder.RegexTokens
{
    public class ComplementToken : RegexSingleCharacterBaseToken
    {
        public ComplementToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Complement;
        }
    }
}