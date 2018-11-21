using Regseed.Parser.TokenParser;

namespace Regseed.Parser.ParserFactories
{
    internal interface IParserFactory
    {
        bool TryGetTokenParser(string character, out ITokenParser tokenParser);
    }
}