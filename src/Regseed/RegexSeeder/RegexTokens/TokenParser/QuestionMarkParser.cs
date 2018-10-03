using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Common.Streams;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens.TokenParser
{
    public class QuestionMarkParser : BaseTokenParser
    {
        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            inputStream.Pop();
            token = new IntegerIntervalToken(new IntegerInterval(0, 1), _initialStreamPosition, 1);
            return new SuccessParseResult();
        }
    }
}