namespace Regseed.Common.Random
{
    public interface IRandomGenerator
    {
        int GetNextInteger(int lower, int upper);
    }
}