using Regseed.Common.Results;
using Regseed.Streams;

namespace Regseed.Parser
{
    internal interface ILexer
    {
        IParserAlphabet ParserAlphabet { get; }
        IParseResult TryConvertToTokenStream(IStringStream inputStream, out ITokenStream tokenStream);
    }
}