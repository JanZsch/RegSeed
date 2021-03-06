using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class TildeParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            token = new ComplementToken(_initialStreamPosition);
            return new SuccessParseResult();
        }
    }
}