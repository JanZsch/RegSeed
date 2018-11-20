using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class OpenSquareBracketParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();

            if (inputStream.CurrentPosition > inputStream.Count || !inputStream.LookAhead(0).Equals(SpecialCharacters.NegateCharacterClass))
            {
                token = new OpenCharacterClassToken(_initialStreamPosition);
                return new SuccessParseResult();   
            }

            inputStream.Pop();
            token = new OpenNegatedCharacterClassToken(_initialStreamPosition);
            return new SuccessParseResult();
        }
    }
}