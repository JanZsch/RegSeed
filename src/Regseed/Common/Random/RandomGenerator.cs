using System;

namespace Regseed.Common.Random
{
    internal class RandomGenerator : IRandomGenerator
    {
        private readonly System.Random _random;
        
        public RandomGenerator(System.Random random)
        {
            _random = random;
        }

        public int GetNextInteger(int lower, int upper)
        {   
            if(lower < 0)
                throw new ArgumentException("Lower bound must be larger or equal to 0.");

            if(upper < lower)
                throw new ArgumentException("Lower bound must be smaller or equal to upper.");

            return -_random.Next(-upper, -lower + 1);
        }
    }
}