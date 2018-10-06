using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    public class AsteriskParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            var interval = new IntegerInterval();
            interval.TrySetValue(0, null);
            token = new IntegerIntervalToken(interval, _initialStreamPosition, 1);
            return new SuccessParseResult();
        }
    }
}