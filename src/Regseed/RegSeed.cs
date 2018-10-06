using System;
using Regseed.Common.Random;
using Regseed.Common.Results;
using Regseed.Expressions;
using Regseed.Factories;
using Regseed.Parser;
using Regseed.Resources;

namespace Regseed
{
    public class RegSeed
    {
        private bool _wasInitialised;
        private readonly IParserAlphabet _alphabet;
        private IExpression _regularExpression;
        protected IRandomGenerator _random;

        public RegSeed(Random random = null, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = new RandomGenerator(random ?? new Random());
        }

        public RegSeed(IRandomGenerator random, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = random ?? new RandomGenerator(new Random());
        }
        
        public RegSeed(string regex, Random random = null, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = new RandomGenerator(random ?? new Random());

            var loadRegexResult = TryLoadRegexPattern(regex);
            
            if (!loadRegexResult.IsSuccess)
                throw new ArgumentException(loadRegexResult.ErrorType.ToExceptionMessage());
        }
      
        public IParseResult TryLoadRegexPattern(string regex)
        {
            regex = regex.TrimStart(SpecialCharacters.StartsWith).TrimEnd(SpecialCharacters.EndsWith);
            var result = new RegularExpressionFactory(_alphabet, _random).TryGetRegularExpression(regex, out var expression);

            if (!result.IsSuccess)
                return result;

            _regularExpression = expression;
            _wasInitialised = true;

            return new SuccessParseResult();
        }

        public string Generate()
        {
            if(!_wasInitialised)
                throw new ArgumentException(RegSeedErrorMessages.InitialiseFirst);
            
            return _regularExpression.ToRegexString();
        }
    }
}