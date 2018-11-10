using NUnit.Framework;

namespace Regseed.Test
{
    [TestFixture]
    public class RegSeedTryLoadPatternTest
    {
        [TestCase("{", 0)]
        [TestCase("a{a}", 1)]
        [TestCase("a-z", 0)]
        [TestCase("[(x)]", 1)]
        [TestCase("(0|1)]", 5)]
        [TestCase("[0|1]", 2)]
        [TestCase("a++", 2)]
        [TestCase("a+*", 2)]
        [TestCase("a{3,1}", 1)]
        [TestCase("[x-a]", 2)]
        [TestCase("a{,1", 1)]
        [TestCase("a{1x,1}", 3)]
        public void TryLoadRegexPattern_ReturnsFailureResultWithExpectedErrorPosition_WhenRegexPatternIsFaulty(string faultyPattern, int expectedErrorPosition)
        {
            var regseed = new RegSeed();
            
            var result = regseed.TryLoadRegexPattern(faultyPattern);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedErrorPosition, result.Position);
        }

    }
}