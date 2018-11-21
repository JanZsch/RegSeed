using Regseed.Parser.TokenParser;

namespace Regseed.Parser.ParserFactories
{
    internal class StatelessParserFactory : IParserFactory
    {
        private readonly IParserAlphabet _alphabet;
        
        public StatelessParserFactory(IParserAlphabet alphabet)
        {
            _alphabet = alphabet;
        }

        public bool TryGetTokenParser(string character, out ITokenParser tokenParser)
        {
            tokenParser = null;
            return _alphabet.IsValid(character) && _alphabet.TryGetTokenParser(character, out tokenParser);
        }
    }
}