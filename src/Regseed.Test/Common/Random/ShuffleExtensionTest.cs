using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Test.TestHelper;

namespace Regseed.Test.Common.Random
{
    [TestFixture]
    public class ShuffleExtensionTest
    {
        [Test]
        public void Shuffle_ChangesAtLeastOnePositionOfListElement_WhenListIs100ElementsLong()
        {
            var integerRange = Enumerable.Range(0, 99).ToList();
            
            integerRange.Shuffle(new RandomGenerator(new System.Random()));

            var wasShifted = integerRange.Where((t, i) => i != t).Any();

            Assert.IsTrue(wasShifted);
        }
        
        [Test]
        public void Shuffle_CreatesEquallyDistributedShuffle_WhenCalled1000TimesOn10ElementList()
        {
            const int shuffleRuns = 30000;
            const int numberOfElements = 10;
            var frequencies = GetInitialisedNullArray(numberOfElements);

            var rand = new RandomGenerator(new System.Random());
            
            for (var i = 0; i < shuffleRuns; i++)
            {
                var range = Enumerable.Range(0, numberOfElements).ToList();
                range.Shuffle(rand);

                for (var j = 0; j < numberOfElements; j++)
                    frequencies[j][range.IndexOf(j)]++;

            }

            var result = StatisticsHelper.IsEquallyDistributed(frequencies, shuffleRuns, 0.05, out var faultyFrequency);
            
            Assert.IsTrue(result, "faulty mapping: {0} -> {1}: derivation: {2}", faultyFrequency?.Item1 ?? -1, faultyFrequency?.Item2 ?? -1, faultyFrequency?.Item3 ?? -1);
        }

        private static List<List<int>> GetInitialisedNullArray(int numberOfElements)
        {
            var frequencies = new List<List<int>>();
            
            var range = Enumerable.Range(0, numberOfElements).ToList();

            foreach (var i in range)
            {
                var list = new List<int>();
                foreach (var j in range)
                    list.Add(0);
                frequencies.Add(list);
            }

            return frequencies;
        }
    }
}