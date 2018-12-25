using System;
using NUnit.Framework;
using Regseed.Common.Random;

namespace Regseed.Test.Common.Random
{
    [TestFixture]
    public class RandomGeneratorTest
    {
        [TestCase(-1)]
        [TestCase(4)]
        public void NextInteger_ThrowsArgumentException_WhenLowerBoundSmallerThanZero(int lower)
        {
            var rand = new RandomGenerator(new System.Random());

            Assert.Throws<ArgumentException>(() => rand.GetNextInteger(lower, 2));
        }
        
        [TestCase(0)]
        [TestCase(12)]
        [TestCase(int.MaxValue)]
        public void NextInteger_ReturnsSpecifiedValue_WhenLowerAndUpperAreEqual(int value)
        {
            var rand = new RandomGenerator(new System.Random());

            var result = rand.GetNextInteger(value, value);
            
            Assert.AreEqual(value, result);
        }
        
        [Test]
        public void NextInteger_ReturnsEachValueWithinIntervalAtLeastOnce_WhenCalledSufficientlyOften()
        {
            const int runs = 100;
            var resultCount = new int[10];
            var rand = new RandomGenerator(new System.Random());

            for (var run = 0; run < runs; run++)
            {
                var result = rand.GetNextInteger(0, 9);
                resultCount[result]++;                
            }

            foreach (var count in resultCount)
                Assert.Greater(count, 0);
        }
    }
}