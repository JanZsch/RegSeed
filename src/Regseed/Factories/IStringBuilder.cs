namespace Regseed.Factories
{
    public interface IStringBuilder
    {
        string GenerateString();
        IStringBuilder ConcatWith(IStringBuilder builder);
        IStringBuilder IntersectWith(IStringBuilder builder);
        IStringBuilder MergeWith(IStringBuilder builder);
    }
}