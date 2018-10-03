namespace Regseed.RegexSeeder.RegexTokens
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
        AnyString,
        OpenPrecedence,
        ClosePrecedence,
        Union,
        EndOfStream
    }
}