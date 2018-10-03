namespace Regseed.RegexSeeder.RegexTokens
{
    public class CloseCharacterClassToken : RegexSingleCharacterBaseToken
    {
        public CloseCharacterClassToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.CloseCharacterClass;
        }
    }
}