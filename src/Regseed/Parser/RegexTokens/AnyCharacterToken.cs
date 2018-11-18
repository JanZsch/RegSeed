namespace Regseed.Parser.RegexTokens
{
    internal class AnyCharacterToken : RegexSingleCharacterBaseToken
    {
        public AnyCharacterToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.AnyCharacter;
        }
    }
}