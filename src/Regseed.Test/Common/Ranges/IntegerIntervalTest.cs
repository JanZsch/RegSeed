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
        public void TrySetValue_ReturnsSuccessResult_WhenLowerBoundSmallerThanUpperBound(int? lowerBound, int? upperBound)
        {
            var interval = new IntegerInterval();

            var result = interval.TrySetValue(lowerBound, upperBound);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(lowerBound, interval.Start);
            Assert.AreEqual(upperBound, interval.End);
        }

        [Test]
        public void TrySetValue_ReturnsFailureResult_WhenLowerBoundLargerThanUpperBound()
        {
            var result = new IntegerInterval().TrySetValue(1, -2);

            Assert.IsFalse(result.IsSuccess);
        }
    }
}