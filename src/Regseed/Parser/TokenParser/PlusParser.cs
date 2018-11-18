using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class PlusParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            var interval = new IntegerInterval();
            interval.TrySetValue(1, null);
            token = new IntegerIntervalToken(interval, _initialStreamPosition, 1);
            return new SuccessParseResult();
        }
    }
}