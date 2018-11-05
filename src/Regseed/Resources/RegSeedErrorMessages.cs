using System.Collections.Generic;

namespace Regseed.Resources
{
    public static class RegSeedErrorMessages
    {
        private static IDictionary<RegSeedErrorType, string> _errorTypeMapper = new Dictionary<RegSeedErrorType, string>
        {
            {RegSeedErrorType.IntegerIntervalSeparatorExpected ,  $"{SpecialCharacters.IntervalSeparator} expected"},
            {RegSeedErrorType.IntegerIntervalExpected ,  "Integer Interval expected."},
            {RegSeedErrorType.SingleCharacterExpected ,  "Single character expected."},
            {RegSeedErrorType.CharacterRangeExpected ,  "Character range expected."},
            {RegSeedErrorType.IntegerExpected ,  "Integer expected."},
            {RegSeedErrorType.InvalidInput ,  "Invalid input."},
            {RegSeedErrorType.UnexpectedEndOfStream ,  "The input stream ended unexpectedly."},
            {RegSeedErrorType.InvalidRange ,  "Invalid character range. The first character has to come before the second one."},
            {RegSeedErrorType.InvalidInterval ,  "Invalid Interval. The lower bound has to be smaller than the upper bound."},
            {RegSeedErrorType.CloseCharacterClassExpected ,  $"{SpecialCharacters.CloseCharacterClass} expected."},
            {RegSeedErrorType.CharacterTypeExpressionExpected,  $"Single character, character range or {SpecialCharacters.AnyCharacter} expected."},
            {RegSeedErrorType.ClosePrecedenceExpected ,  $"{SpecialCharacters.ClosePrecedence} expected."},
            {RegSeedErrorType.UnionExpressionExpected,  $"{SpecialCharacters.Or} expected."}            
        };
        
        public const string RegexTypeExpected = "GetType expected to be called only with Enum-Type RegexTokenType";
        public const string StreamInitNotCalled = "InitStreamElements not called.";
        public const string AddCharacterTokenParserError = "The input key has to be of length 1.";
        public const string CharacterTokenParserDuplicate = "The provided letter was already added.";
        public const string InitialiseFirst = "RegSeed requires initialisation with regex pattern first.";
        public const string InvalidNumberOfConcatenations = "The number of subsequent concatenations must be positive...";

        
        public static string ToExceptionMessage(this RegSeedErrorType errorType)
        {
            return _errorTypeMapper.TryGetValue(errorType, out var exceptionMessage) 
                ? exceptionMessage 
                : "Shoot. An Unknown Error occurred...";
        }
    }
}