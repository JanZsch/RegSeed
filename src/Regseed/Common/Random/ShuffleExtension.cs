using System.Collections.Generic;

namespace Regseed.Common.Random
{
    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this IList<T> list, IRandomGenerator rand)  
        {  
            var n = list.Count;
            while (n-- > 0)
            {
                var k = rand.GetNextInteger(0, n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }
        }
    }
}