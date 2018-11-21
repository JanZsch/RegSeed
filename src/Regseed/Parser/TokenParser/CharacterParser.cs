using Regseed.Common.Results;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.RegexTokens;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Parser.TokenParser
{
    internal class CharacterParser : BaseCharacterParser
    {
        public CharacterParser(IPrimitiveParser primitiveParser) : base(primitiveParser)
        {
        }

        protected override bool EnforceCharacterRangeSeparatorEscaping => false;
    }
}