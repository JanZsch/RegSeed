using System;
using NUnit.Framework;
using Regseed.Common.Ranges;

namespace Regseed.Test.Common.Ranges
{
    [TestFixture]
    public class IntegerIntervalTest
    {
        [TestCase(null, null)]
        [TestCase(null, -2)]
        [TestCase(2, null)]
        public void Constructor_DoesNotThrow_WhenLowerBoundSmallerThanUpperBound(int? lowerBound, int? upperBound)
        {
            Assert.DoesNotThrow(() => _ = new IntegerInterval(lowerBound, upperBound));
        }

        [Test]
        public void Constructor_ThrowsArgumentException_WhenLowerBoundLargerThanUpperBound()
        {
            Assert.Throws<ArgumentException>(() => _ = new IntegerInterval(1, -2));
        }
    }
}