namespace Regseed.Parser.RegexTokens
{
    internal class IntersectionToken : RegexSingleCharacterBaseToken
    {
        public IntersectionToken(long position) : base(position)
        {
            _tokenType = RegexTokenType.Intersection;
        }
    }
}