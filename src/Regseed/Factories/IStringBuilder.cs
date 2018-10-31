namespace Regseed.Factories
{
    public interface IStringBuilder
    {
        int GeneratedStringLength();
        string GenerateString();
        IStringBuilder ConcatWith(IStringBuilder builder);
        IStringBuilder IntersectWith(IStringBuilder builder);
        IStringBuilder MergeWith(IStringBuilder builder);
    }
}