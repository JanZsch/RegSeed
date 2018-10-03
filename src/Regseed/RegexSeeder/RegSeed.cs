using System;
using Regseed.Common.Parser;
using Regseed.Common.Random;
using Regseed.Common.Results;
using Regseed.RegexSeeder.Expressions;
using Regseed.RegexSeeder.Factories;
using Regseed.RegexSeeder.Resources;

namespace Regseed.RegexSeeder
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

        public RegSeed(string regex, Random random = null, IParserAlphabet parserAlphabet = null)
        {
            _alphabet = parserAlphabet ?? RegexAlphabetFactory.Default();
            _random = new RandomGenerator(random ?? new Random());

            if (!TryLoadRegexPattern(regex).IsSuccess)
                throw new ArgumentException(InterpreterMessages.InvalidRegex);
        }
              
        public IParseResult TryLoadRegexPattern(string regex)
        {
            regex = regex.TrimStart('^').TrimEnd('$');
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
                throw new ArgumentException(RegSeedMessages.InitialiseFirst);
            
            return _regularExpression.ToRegexString();
        }
    }
}