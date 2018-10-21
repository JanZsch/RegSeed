namespace Regseed.Parser.RegexTokens
{
    public enum RegexTokenType
    {
        Character,
        CharacterRange,
        IntegerRange,
        OpenCharacterClass,
        CloseCharacterClass,
        OpenNegateCharacterClass,
        Complement,
        AnyCharacter,
        OpenPrecedence,
        ClosePrecedence,
        Union,
        Intersection,
        EndOfStream
    }
}