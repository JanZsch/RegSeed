namespace Regseed.Parser.RegexTokens
{
    public class IntersectionToken : RegexSingleCharacterBaseToken
    {
        public IntersectionToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Intersection;
        }
    }
}