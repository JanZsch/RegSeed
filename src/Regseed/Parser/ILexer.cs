using Regseed.Common.Results;
using Regseed.Streams;

namespace Regseed.Parser
{
    internal interface ILexer
    {
        IParseResult TryConvertToTokenStream(string pattern, out ITokenStream tokenStream);
        IParseResult TryConvertToTokenStream(IStringStream inputStream, out ITokenStream tokenStream);
    }
}