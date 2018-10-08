using System.Collections.Generic;
using System.Linq;

namespace Regseed
{
    public static class RegSeedExtensions
    {
        private static readonly IDictionary<string, string> _wildCardRegexMapper;

        static RegSeedExtensions()
        {
            const char newLineCharacter = (char) 0xA;
            const char tabStop = (char)0x9;
            const char space = ' ';
            var whitespaceRegex = $"[{newLineCharacter}{tabStop}{space}]";
            var nonWhitespaceRegex = $"[^{newLineCharacter}{tabStop}{space}]";

            _wildCardRegexMapper = new Dictionary<string, string>
            {
                {"\\w", "[a-zA-Z0-9_]"},
                {"\\W", "[^a-zA-Z0-9_]"},
                {"\\d", "[0-9]"},
                {"\\s", whitespaceRegex},
                {"\\S", nonWhitespaceRegex},
                {"\\D", "[^0-9]"}
            };
        }

        public static string ReplaceRegexWildCards(this string pattern)
        {
            return pattern == null 
                ? null 
                : _wildCardRegexMapper.Aggregate(pattern, (current, wildCardRegexPair) => current.Replace(wildCardRegexPair.Key, wildCardRegexPair.Value));
        }
    }
}