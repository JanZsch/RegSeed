using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    public interface ITokenParser
    {
        IParseResult TryGetToken(IStringStream inputStream, out IToken token);
    }
}