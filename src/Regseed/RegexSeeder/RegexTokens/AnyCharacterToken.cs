namespace Regseed.RegexSeeder.RegexTokens
{
    public class AnyCharacterToken : RegexSingleCharacterBaseToken
    {
        public AnyCharacterToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.AnyCharacter;
        }
    }
}