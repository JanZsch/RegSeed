using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Factories;
using Regseed.Parser;

namespace Regseed.Test.Factories
{
    [TestFixture]
    public class RegularExpressionFactoryTest
    {
        [TestCase("x|y&1")]
        [TestCase("y&1")]
        [TestCase("y([1]&2)1")]
        [TestCase("1(1|2|[3-4]&3)0")]
        public void TryLoadRegex_ResultValueHasIntersectionIsTrue_WhenPatternContainsIntersectionToken(string pattern)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>());
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.IsTrue(result.Value.HasIntersection);
        }
        
        [TestCase("")]
        public void TryLoadRegex_ResultValueHasIntersectionIsFalse_WhenPatternDoesNotContainIntersectionToken(string pattern)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>());
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.IsFalse(result.Value.HasIntersection);
        }

    }
}