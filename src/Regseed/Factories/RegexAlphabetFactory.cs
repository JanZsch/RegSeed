using System.Collections.Generic;
using Regseed.Parser;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.TokenParser;

namespace Regseed.Factories
{
    public static class RegexAlphabetFactory
    {
        public static IParserAlphabet Minimal(bool areRegexControlCharactersValid = true)
        {
            var parserAlphabet = new ParserAlphabet();
            var primitiveParser = new PrimitiveParser(parserAlphabet);

            parserAlphabet.AddRegexControlCharacters(primitiveParser, areRegexControlCharactersValid);

            return parserAlphabet;
        }
        
        public static IParserAlphabet Default(bool areRegexControlCharactersValid = true)
        {
            var defaultAlphabet = Minimal(areRegexControlCharactersValid);
            var primitiveParser = new PrimitiveParser(defaultAlphabet);

            defaultAlphabet.AddLetters(primitiveParser)
                           .AddCapitalLetters(primitiveParser)
                           .AddDigits(primitiveParser)
                           .AddWhitespaces(primitiveParser)
                           .AddSpecialCharacters(primitiveParser);

            return defaultAlphabet;
        }
        
        public static IParserAlphabet DefaultExtendedBy(IEnumerable<string> additionalValidCharacters, bool areRegexControlCharactersValid = true)
        {
            var defaultAlphabet = Default(areRegexControlCharactersValid);
            var primitiveParser = new PrimitiveParser(defaultAlphabet);

            foreach (var additionalLetter in additionalValidCharacters)
                defaultAlphabet.Add(additionalLetter, new CharacterParser(primitiveParser));
            
            return defaultAlphabet;
        }

        public static IParserAlphabet MinimalExtendedBy(IEnumerable<string> additionalValidCharacters, bool areRegexControlCharactersValid = true)
        {
            var minimalAlphabet = Minimal(areRegexControlCharactersValid);
            var primitiveParser = new PrimitiveParser(minimalAlphabet);

            foreach (var additionalLetter in additionalValidCharacters)
                minimalAlphabet.Add(additionalLetter, new CharacterParser(primitiveParser));
            
            return minimalAlphabet;
        }
    }
}