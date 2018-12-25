namespace Regseed.Common.Random
{
    public interface IRandomGenerator
    {
        /// <summary>Generates random integer number larger or equal to lower and smaller or equal to upper. The largest possible range is [0, int.MaxValue]</summary>
        /// <exception cref="System.ArgumentException">Thrown when lower is smaller than 0.</exception>
        /// <exception cref="System.ArgumentException">Thrown when lower is larger than upper.</exception>
        int GetNextInteger(int lower, int upper);
    }
}