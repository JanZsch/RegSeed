using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class RegularExpressionFactoryTest
    {
        [TestCase("x|y&1", true)]
        [TestCase("y&1", true)]
        [TestCase("y([1]&2)1", true)]
        [TestCase("1(1|2|[3-4]&3)0", true)]
        [TestCase("1(1|2|[3-4]3)0", false)]
        [TestCase("1(1{1,2}[^abs])0", false)]
        public void TryLoadRegex_ResultValueHasIntersectionIsExpectedValue_DependingOnWhethernPatternContainsIntersectionTokenOrNot(string pattern, bool expectedResult)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.AreEqual(expectedResult, result.Value.HasIntersection);
        }
        
        [TestCase("~1", true)]
        [TestCase("x(~(12)|A)y", true)]
        [TestCase("x((12)|A)y", false)]
        [TestCase("x([^asd]y{0,12})y", false)]
        public void TryLoadRegexPattern_ContainsComplementHasExpectedValue_DependingOnWhetherPatternContainsComplementOrNot(string pattern, bool expectedResult)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.AreEqual(expectedResult, result.Value.HasComplement);
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
        [TestCase("a?&b{8}", 0)]
        [TestCase("a{0,2}|b{3}|franzi", 6)]
        [TestCase("Ul|rik|e", 3)]
        [TestCase("U(l{0,1}|r{2}|ike){1,2}!{1,4}", 11)]
        [TestCase("U(l{0,1}|r{2}|[Ii]ke&ike){1,2}!{1,4}", 11)]
        [TestCase("[^1]", 1)]
        [TestCase(".*&a", 1)]
        [TestCase("a{3,4}&c{1,2}", 0)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMaxExpansionLength_WhenCalledForPattern(string pattern, int expectedMaxExpansionLength)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);

            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionRange.End;
            
            Assert.AreEqual(expectedMaxExpansionLength, result);
        }

        [TestCase("~(Trump)", 5+2)]
        [TestCase("~1", 1+2)]
        [TestCase("~1&12&3", 0)]
        [TestCase("~1&12&34", 2)]
        [TestCase("~1|12&3", 1+2)]
        public void TryLoadRegex_ReturnedMaxExpansionLengthIsMaxLengthOfInvertedExpressionPlusInverseLengthOffset_WhenPatternContainsInverse(string pattern, int expectedMaxExpansionLength)
        {
            const int inverseLengthOffset = 2;
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), inverseLengthOffset);


            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionRange.End;
            
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
        [TestCase("~(no)", 0)]
        [TestCase("~1&12&3", 0)]
        [TestCase("~1|12&3", 0)]
        [TestCase(".*&a", 1)]
        [TestCase(".+&ab", 2)]
        [TestCase("a{3,4}&c{1,2}", 0)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMinExpansionLength_WhenCalledForPattern(string pattern, int expectedMinExpansionLength)
        {
            var factory = new RegularExpressionFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);

            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionRange.Start;
            
            Assert.AreEqual(expectedMinExpansionLength, result);
        }
    }
}