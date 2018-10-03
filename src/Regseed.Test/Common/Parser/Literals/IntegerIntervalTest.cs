using System;
using NUnit.Framework;
using Regseed.Common.Ranges;

namespace Regseed.Test.Common.Parser.Literals
{
    [TestFixture]
    public class IntegerIntervalTest
    {
        [Test]
        public void Constructor_ThrowsArgumentException_WhenLowerBoundLargerThanUpperBound()
        {
            Assert.Throws<ArgumentException>(() => _ = new IntegerInterval(1, -2));
        }
        
        [TestCase(null, null)]
        [TestCase(null, -2)]
        [TestCase(2, null)]
        public void Constructor_DoesNotThrow_WhenLowerBoundSmallerThanUpperBound(int? lowerBound, int? upperBound)
        {
            Assert.DoesNotThrow(() => _ = new IntegerInterval(lowerBound, upperBound));
        }
    }
}