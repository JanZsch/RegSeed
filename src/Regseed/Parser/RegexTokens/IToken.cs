namespace Regseed.Parser.RegexTokens
{
    public interface IToken
    {
        long Position { get; }
        int Length { get; }
        TEnum GetType<TEnum>() where TEnum : struct;
        TValue GetValue<TValue>();
    }
}