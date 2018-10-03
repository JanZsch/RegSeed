namespace Regseed.Common.Random
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly System.Random _random;
        
        public RandomGenerator(System.Random random)
        {
            _random = random;
        }

        public int GetNextInteger(int lower, int upper)
        {
            return _random.Next(lower, upper);
        }
    }
}