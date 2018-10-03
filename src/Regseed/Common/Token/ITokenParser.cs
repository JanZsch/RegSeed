using Regseed.Common.Results;
using Regseed.Common.Streams;

namespace Regseed.Common.Token
{
    public interface ITokenParser
    {
        IParseResult TryGetToken(IStringStream inputStream, out IToken token);
    }
}