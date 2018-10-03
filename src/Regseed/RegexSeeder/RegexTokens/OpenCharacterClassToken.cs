namespace Regseed.RegexSeeder.RegexTokens
{
    public class OpenCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public OpenCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.OpenCharacterClass;
        }
    }
}