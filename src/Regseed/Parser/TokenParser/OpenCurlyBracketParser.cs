using Regseed.Common.Results;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class OpenCurlyBracketParser : BaseTokenParser
    {
        private readonly IPrimitiveParser _primitiveParser;

        public OpenCurlyBracketParser(IPrimitiveParser primitiveParser)
        {
            _primitiveParser = primitiveParser;
        }

        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            token = null;
            
            var intervalResult = _primitiveParser.TryParseIntegerInterval(inputStream);
            if (!intervalResult.IsSuccess) 
                return intervalResult;

            var rangeTokenLength = (int) (inputStream.CurrentPosition - _initialStreamPosition);
            token = new IntegerIntervalToken(intervalResult.Value, inputStream.CurrentPosition, rangeTokenLength);
            return new SuccessParseResult();
        }
    }
}