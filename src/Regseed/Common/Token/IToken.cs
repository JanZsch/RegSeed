namespace Regseed.Common.Token
{
    public interface IToken
    {
        TEnum GetType<TEnum>() where TEnum : struct;
        TValue GetValue<TValue>();
        long Position { get; }
        int Length { get; }
    }
}