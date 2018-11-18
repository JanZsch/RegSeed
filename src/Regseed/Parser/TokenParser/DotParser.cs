using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class DotParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            token = new AnyCharacterToken(_initialStreamPosition);
            return new SuccessParseResult();
        }
    }
}