namespace Regseed.Resources
{
    public enum RegSeedErrorType
    {
        None,
        UnionExpressionExpected,
        CharacterTypeExpressionExpected,
        ClosePrecedenceExpected,
        CloseCharacterClassExpected,
        CharacterRangeExpected,
        IntegerIntervalExpected,
        IntegerIntervalSeparatorExpected,
        SingleCharacterExpected,
        UnexpectedEndOfStream,
        IntegerExpected,
        InvalidInput,
        InvalidRange,
        InvalidInterval        
    }
}