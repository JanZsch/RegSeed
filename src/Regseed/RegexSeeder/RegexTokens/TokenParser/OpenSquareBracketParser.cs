using Regseed.Common.Resources;
using Regseed.Common.Results;
using Regseed.Common.Streams;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens.TokenParser
{
    public class OpenSquareBracketParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();

            if (!inputStream.LookAhead(0).Equals(SpecialCharacters.NegateCharacterClass))
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