using Regseed.Common.Parser;
using Regseed.Common.Results;
using Regseed.Common.Streams;
using Regseed.Common.Token;

namespace Regseed.RegexSeeder.RegexTokens.TokenParser
{
    public class CharacterParser : BaseTokenParser
    {
        private readonly IPrimitiveParser _primitiveParser;

        public CharacterParser(IPrimitiveParser primitiveParser)
        {
            _primitiveParser = primitiveParser;
        }

        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            token = null;
            
            var rangeResult = _primitiveParser.TryParseCharacterRange(inputStream);
            if (rangeResult.IsSuccess)
            {
                var rangeTokenLength = (int) (inputStream.CurrentPosition - _initialStreamPosition);
                token = new CharacterRangeToken(rangeResult.Value, inputStream.CurrentPosition, rangeTokenLength);
                return new SuccessParseResult();
            }

            var characterResult = _primitiveParser.TryParseCharacter(inputStream);
            if (!characterResult.IsSuccess)
                return characterResult;

            var characterTokenLength = (int) (inputStream.CurrentPosition - _initialStreamPosition);
            token = new CharacterToken(characterResult.Value, inputStream.CurrentPosition, characterTokenLength);
            return new SuccessParseResult();
        }
    }
}