using System;
using System.Linq;
using Regseed.Common.Random;
using Regseed.Common.Results;
using Regseed.Expressions;
using Regseed.Parser;
using Regseed.Resources;

namespace Regseed
{
    public class RegSeed
    {
        private const int _defaultCharClassInverseLength = 5;
        
        private bool _wasInitialised;
        private bool _replaceWildCards;
        private readonly IParserAlphabet _alphabet;
        private ExpressionMetaData _expressionMetaData;
        private IExpression _regularExpression;
        protected IRandomGenerator _random;

        public int InverseLengthOffset { get; private set; }
        
        public RegSeed(Random random = null, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = new RandomGenerator(random ?? new Random());
            InverseLengthOffset = _defaultCharClassInverseLength;
            _replaceWildCards = false;
        }

        public RegSeed(IRandomGenerator random, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = random ?? new RandomGenerator(new Random());
            InverseLengthOffset = _defaultCharClassInverseLength;
            _replaceWildCards = false;
        }

        public RegSeed(string regex, Random random = null, IParserAlphabet parserAlphabet = null)
        {
            _replaceWildCards = false;
            InverseLengthOffset = _defaultCharClassInverseLength;
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = new RandomGenerator(random ?? new Random());

            var loadRegexResult = TryLoadRegexPattern(regex);

            if (!loadRegexResult.IsSuccess)
                throw new ArgumentException(loadRegexResult.ErrorType.ToExceptionMessage());
        }

        public IParseResult TryLoadRegexPattern(string regex)
        {
            _wasInitialised = false;
            _expressionMetaData = null;
            _regularExpression = null;

            if (_replaceWildCards)
                regex = regex.ReplaceRegexWildCards();

            regex = regex.TrimStart(SpecialCharacters.StartsWith).TrimEnd(SpecialCharacters.EndsWith);

            var regexFactory = new RegularExpressionFactory(_alphabet, _random, InverseLengthOffset);

            var result = regexFactory.TryGetRegularExpression(regex, out var expression);

            if (!result.IsSuccess)
                return result;

            _regularExpression = expression;
            _expressionMetaData = result.Value;
            _wasInitialised = true;

            return new SuccessParseResult();
        }

        public RegSeed EnableStandardWildCards()
        {
            if (_wasInitialised)
                throw new ArgumentException(RegSeedErrorMessages.CallBeforeInitialisation);

            _replaceWildCards = true;

            return this;
        }

        public RegSeed SetInverseLengthOffset(int inverseLengthOffset)
        {
            if(inverseLengthOffset < 1)
                throw new ArgumentException(RegSeedErrorMessages.InverseLengthOffsetOutOfRange);
            
            InverseLengthOffset = inverseLengthOffset;

            return this;
        }

        public string Generate()
        {
            if (!_wasInitialised)
                throw new ArgumentException(RegSeedErrorMessages.InitialiseFirst);

            if (!_expressionMetaData.HasIntersection && !_expressionMetaData.HasComplement)
                return _regularExpression.ToStringBuilder().GenerateString();
            
            var expandedStringBuilder = _regularExpression.Expand();

            if (!expandedStringBuilder.Any()) 
                return string.Empty;

            var random = _random.GetNextInteger(0, expandedStringBuilder.Count - 1);
            return expandedStringBuilder[random].GenerateString();
        }
    }
}