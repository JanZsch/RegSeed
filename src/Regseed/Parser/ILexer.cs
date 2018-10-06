using Regseed.Common.Results;
using Regseed.Streams;

namespace Regseed.Parser
{
    public interface ILexer
    {
        IParserAlphabet ParserAlphabet { get; }
        IParseResult TryCreateTokenStream(IStringStream inputStream, out ITokenStream tokenStream);
    }
}