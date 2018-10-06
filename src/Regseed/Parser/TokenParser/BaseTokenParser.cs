using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    public abstract class BaseTokenParser : ITokenParser
    {
        protected long _initialStreamPosition;

        public IParseResult TryGetToken(IStringStream inputStream, out IToken token)
        {
            token = null;

            if (inputStream == null)
                return new FailureParseResult(0, RegSeedErrorType.InvalidInput);

            _initialStreamPosition = inputStream.CurrentPosition;
            
            return TryGetTokenWithoutNullCheck(inputStream, out token);
        }

        protected abstract IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token);
    }
}