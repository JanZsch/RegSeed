namespace Regseed.Resources
{
    internal static class InterpreterMessages
    {
        public const string InvalidRegex = "The provided Regex was invalid. You might instead consider calling TryLoadPattern on the parameterless constructor.";
        public const string CloseCharacterClassExpected = "']' expected.";
        public const string CharacterAnyCharacterOrCharacterRangeExpected = "Single character, character range or '.' expected.";
        public const string UnknownLetter = "The provided letter '{0}' is no element of the provided alphabet.";
        public const string ClosePrecedenceExpected = "')' expected.";
        public const string UnionExpected = "'|' expected.";
    }
}