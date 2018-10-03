using Regseed.Common.Parser.Settings;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Common.Streams;

namespace Regseed.Common.Parser
{
    public interface IPrimitiveParser
    {
        IParseResult<string> TryParseCharacter(IStringStream stream);
        IParseResult<CharacterRange> TryParseCharacterRange(IStringStream stream, string rangeSeparator = null);
        IParseResult<int> TryParseInteger(IStringStream stream);
        IParseResult<IntegerInterval> TryParseIntegerInterval(IStringStream stream, ParseIntervalSettings settings = null);
    }
}