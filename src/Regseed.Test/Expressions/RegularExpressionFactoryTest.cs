using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
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
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.IsTrue(result.Value.HasIntersection);
        }
        
        [TestCase("")]
        public void TryLoadRegex_ResultValueHasIntersectionIsFalse_WhenPatternDoesNotContainIntersectionToken(string pattern)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.IsFalse(result.Value.HasIntersection);
        }        
        
        [TestCase("a", 1)]
        [TestCase("a{0,2}b", 3)]
        [TestCase("a{0,2}b{8}", 10)]
        [TestCase("a*b{8}", int.MaxValue)]
        [TestCase("a+b{8}", int.MaxValue)]
        [TestCase("a?&b{8}", 1)]
        [TestCase("a{0,2}|b{3}|franzi", 6)]
        [TestCase("Ul|rik|e", 3)]
        [TestCase("U(l{0,1}|r{2}|ike){1,2}!{1,4}", 11)]
        [TestCase("U(l{0,1}|r{2}|[Ii]ke&ike){1,2}!{1,4}", 11)]
        [TestCase("~(Trump)", int.MaxValue)]
        [TestCase("[^1]", 1)]
        [TestCase("~1", int.MaxValue)]
        [TestCase("~1&12&3", 1)]
        [TestCase("~1|12&3", int.MaxValue)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMaxExpansionLength_WhenCalledForPattern(string pattern, int expectedMaxExpansionLength)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);

            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionInterval.End;
            
            Assert.AreEqual(expectedMaxExpansionLength, result);
        }
        
        [TestCase("a", 1)]
        [TestCase("a{0,2}b", 1)]
        [TestCase("a{0,2}b{8}", 8)]
        [TestCase("a*b{8}", 8)]
        [TestCase("a+b{8}", 9)]
        [TestCase("a?&b{8}", 0)]
        [TestCase("a{0,2}|b{3}|franzi", 0)]
        [TestCase("Ul|rik|e", 1)]
        [TestCase("U(l{0,1}|r{2}|ike){1,2}!{1,4}", 2)]
        [TestCase("U(l{0,1}|r{2}|[Ii]ke&ike){1,2}!{1,4}", 2)]
        [TestCase("~(Trump)", 0)]
        [TestCase("[^1]", 1)]
        [TestCase("~1", 0)]
        [TestCase("~1&12&3", 0)]
        [TestCase("~1|12&3", 0)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMinExpansionLength_WhenCalledForPattern(string pattern, int expectedMinExpansionLength)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);

            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionInterval.Start;
            
            Assert.AreEqual(expectedMinExpansionLength, result);
        }
    }
}