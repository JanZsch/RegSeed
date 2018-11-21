using Regseed.Common.Results;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal abstract class BaseCharacterParser : BaseTokenParser
    {
        private readonly IPrimitiveParser _primitiveParser;

        protected BaseCharacterParser(IPrimitiveParser primitiveParser)
        {
            _primitiveParser = primitiveParser;
        }

        protected abstract bool EnforceCharacterRangeSeparatorEscaping { get; }

        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            token = null;

            if(EnforceCharacterRangeSeparatorEscaping && IsCharacterRangeSeparator(inputStream))
                return new FailureParseResult(inputStream.CurrentPosition, RegSeedErrorType.InvalidRange);
            
            var rangeResult = _primitiveParser.TryParseCharacterRange(inputStream);
            if (rangeResult.IsSuccess)
            {              
                var rangeTokenLength = (int) (inputStream.CurrentPosition - _initialStreamPosition);
                token = new CharacterRangeToken(rangeResult.Value, inputStream.CurrentPosition, rangeTokenLength);
                return new SuccessParseResult();
            }

            if (!rangeResult.IsSuccess && rangeResult.ErrorType != RegSeedErrorType.None)
                return rangeResult;

            var characterResult = _primitiveParser.TryParseCharacter(inputStream);
            if (!characterResult.IsSuccess)
                return characterResult;

            var characterTokenLength = (int) (inputStream.CurrentPosition - _initialStreamPosition);
            token = new CharacterToken(characterResult.Value, inputStream.CurrentPosition, characterTokenLength);
            return new SuccessParseResult();
        }

        private static bool IsCharacterRangeSeparator(IStringStream inputStream) =>
            inputStream.LookAhead(0).Equals(SpecialCharacters.CharacterRangeSeparator);
    }
}