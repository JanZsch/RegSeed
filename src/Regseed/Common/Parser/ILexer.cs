using Regseed.Common.Results;
using Regseed.Common.Streams;

namespace Regseed.Common.Parser
{
    public interface ILexer
    {
        IParserAlphabet ParserAlphabet { get; }
        IParseResult TryCreateTokenStream(IStringStream inputStream, out ITokenStream tokenStream);
    }
}