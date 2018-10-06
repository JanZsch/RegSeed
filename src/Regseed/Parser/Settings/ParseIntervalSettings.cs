using Regseed.Resources;

namespace Regseed.Parser.Settings
{
    public class ParseIntervalSettings
    {
        public string OpenSymbol { get; set; }
        public string CloseSymbol { get; set; }
        public string Separator { get; set; }

        public static ParseIntervalSettings Default => new ParseIntervalSettings
        {
            Separator = SpecialCharacters.IntervalSeparator,
            CloseSymbol = SpecialCharacters.CloseInterval,
            OpenSymbol = SpecialCharacters.OpenInterval
        };
    }
}