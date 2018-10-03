using Regseed.Common.Resources;
using Regseed.Common.Results;
using Regseed.Common.Streams;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens.TokenParser
{
    public abstract class BaseTokenParser : ITokenParser
    {
        protected long _initialStreamPosition;

        public IParseResult TryGetToken(IStringStream inputStream, out IToken token)
        {
            token = null;

            if (inputStream == null)
                return new FailureParseResult(0, ParserMessages.InvalidInput);

            _initialStreamPosition = inputStream.CurrentPosition;
            
            return TryGetTokenWithoutNullCheck(inputStream, out token);
        }

        protected abstract IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token);
    }
}