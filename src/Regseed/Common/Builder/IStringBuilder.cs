namespace Regseed.Common.Builder
{
    public interface IStringBuilder
    {
        int GeneratedStringLength();
        string GenerateString();
        IStringBuilder ConcatWith(IStringBuilder builder, int times = 1);
        IStringBuilder IntersectWith(IStringBuilder builder);
    }
}