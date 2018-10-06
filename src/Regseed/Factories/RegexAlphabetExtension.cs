using Regseed.Parser;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.TokenParser;
using Regseed.Resources;

namespace Regseed.Factories
{
    public static class RegexAlphabetExtension
    {
        public static void AddRegexControlCharacters(this IParserAlphabet alphabet, IPrimitiveParser parser, bool areRegexSyntaxCharactersValid = true)
        {
            alphabet.Add(SpecialCharacters.ArbitraryMany, new AsteriskParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.AtLeastOne, new PlusParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.OpenInterval, new OpenCurlyBracketParser(parser), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.AtMostOne, new QuestionMarkParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.OpenCharacterClass, new OpenSquareBracketParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.CloseCharacterClass, new CloseSquareBracketParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.OpenPrecedence, new OpenRoundBracketParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.ClosePrecedence, new CloseRoundBracketParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.Or, new PipeParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.AnyCharacter, new DotParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.Complement, new TildeParser(), areRegexSyntaxCharactersValid)
                    .Add(SpecialCharacters.Escape, new CharacterParser(parser), areRegexSyntaxCharactersValid);
        }

        public static IParserAlphabet AddSpecialCharacters(this IParserAlphabet alphabet, IPrimitiveParser parser)
        {
            alphabet.Add("^", new CharacterParser(parser))
                    .Add("&", new CharacterParser(parser))
                    .Add("}", new CharacterParser(parser))
                    .Add("\"", new CharacterParser(parser))
                    .Add("!", new CharacterParser(parser))
                    .Add("$", new CharacterParser(parser))
                    .Add("%", new CharacterParser(parser))
                    .Add("'", new CharacterParser(parser))
                    .Add(",", new CharacterParser(parser))
                    .Add("/", new CharacterParser(parser))
                    .Add(":", new CharacterParser(parser))
                    .Add(";", new CharacterParser(parser))
                    .Add("<", new CharacterParser(parser))
                    .Add(">", new CharacterParser(parser))
                    .Add("_", new CharacterParser(parser))
                    .Add("-", new CharacterParser(parser))
                    .Add("#", new CharacterParser(parser))
                    .Add("=", new CharacterParser(parser))
                    .Add("§", new CharacterParser(parser))
                    .Add("@", new CharacterParser(parser));

            return alphabet;
        }

        public static IParserAlphabet AddLetters(this IParserAlphabet alphabet, IPrimitiveParser parser)
        {
            for (var letter = 'a'; letter <= 'z'; letter++)
                alphabet.Add(letter.ToString(), new CharacterParser(parser));

            return alphabet;
        }

        public static IParserAlphabet AddCapitalLetters(this IParserAlphabet alphabet, IPrimitiveParser parser)
        {
            for (var capitalLetter = 'A'; capitalLetter <= 'Z'; capitalLetter++)
                alphabet.Add(capitalLetter.ToString(), new CharacterParser(parser));

            return alphabet;
        }

        public static IParserAlphabet AddDigits(this IParserAlphabet alphabet, IPrimitiveParser parser)
        {
            for (var digit = 0; digit <= 9; digit++)
                alphabet.Add(digit.ToString(), new CharacterParser(parser));

            return alphabet;
        }
    }
}