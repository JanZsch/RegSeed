using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class QuestionMarkParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            var interval = new IntegerInterval();
            interval.TrySetValue(0, 1);
            token = new IntegerIntervalToken(interval, _initialStreamPosition, 1);
            return new SuccessParseResult();
        }
    }
}