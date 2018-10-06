using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Parser.Settings;
using Regseed.Streams;

namespace Regseed.Parser.PrimitiveParsers
{
    public interface IPrimitiveParser
    {
        IParseResult<string> TryParseCharacter(IStringStream stream);
        IParseResult<CharacterRange> TryParseCharacterRange(IStringStream stream, string rangeSeparator = null);
        IParseResult<int> TryParseInteger(IStringStream stream);
        IParseResult<IntegerInterval> TryParseIntegerInterval(IStringStream stream, ParseIntervalSettings settings = null);
    }
}