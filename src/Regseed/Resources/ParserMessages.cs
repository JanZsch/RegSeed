namespace Regseed.Resources
{
    public static class ParserMessages
    {
        public const string RegexTypeExpected = "GetType expected to be called only with Enum-Type RegexTokenType";
        public const string IntegerIntervalSeparatorExpected = "',' expected";
        public const string IntegerIntervalExpected = "Integer Intervall expected.";
        public const string SingleCharacterExpected = "Single character expected.";
        public const string CharacterRangeExpected = "Character range expected.";
        public const string IntegerExpected = "Integer expected.";
        public const string InvalidInput = "Invalid input.";
        public const string StreamInitNotCalled = "InitStreamElements not called.";
        public const string AddLetterTokenParserError = "The input key has to be of length 1.";
        public const string LetterTokenParserDuplicate = "The provided letter was already added.";
        public const string UnexpectedEndOfStream = "The input stream ended unexcpectetly.";
        public const string InvalidRange = "Invalid Range {0}-{1}. The letter {0} comes after {1}.";
        public const string InvalidInterval = "Invalid Interval {0}-{1}. The lower bound has to be smaller than the upper bound";
    }
}