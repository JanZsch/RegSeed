using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;
using Regseed.Parser;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class RegularExpressionFactoryTest
    {
        [SetUp]
        public void SetUp()
        {
            RegularExpressionFactory.Reset();
        }

        [Test]
        public void GetFactorySingleton_ThrowsArgumentException_WhenFactoryWasNotInitialisedFirst()
        {
            Assert.Throws<ArgumentException>(() => RegularExpressionFactory.GetFactoryAsSingleton());
        }

        [Test]
        public void GetFactorySingleton_ReturnsFactoryInstanceAsSingleton()
        {
            RegularExpressionFactory.InitFactory(Substitute.For<IParserAlphabet>(), Substitute.For<IRandomGenerator>(), 2);
            
            var result1 = RegularExpressionFactory.GetFactoryAsSingleton();
            var result2 = RegularExpressionFactory.GetFactoryAsSingleton();
            
            Assert.AreEqual(result1, result2);
        }

        
        [TestCase("x|y&1")]
        [TestCase("y&1")]
        [TestCase("y([1]&2)1")]
        [TestCase("1(1|2|[3-4]&3)0")]
        public void TryLoadRegex_ResultValueHasIntersectionIsTrue_WhenPatternContainsIntersectionToken(string pattern)
        {
            RegularExpressionFactory.InitFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            var factory = RegularExpressionFactory.GetFactoryAsSingleton();
            
            var result = factory.TryGetRegularExpression(pattern, out _);
            
            Assert.IsTrue(result.Value.HasIntersection);
        }
        
        [TestCase("")]
        public void TryLoadRegex_ResultValueHasIntersectionIsFalse_WhenPatternDoesNotContainIntersectionToken(string pattern)
        {
            RegularExpressionFactory.InitFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            var factory = RegularExpressionFactory.GetFactoryAsSingleton();

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
        [TestCase("~(Trump)", int.MaxValue)]
        [TestCase("[^1]", 1)]
        [TestCase("~1", int.MaxValue)]
        [TestCase("~1&12&3", 0)]
        [TestCase("~1&12&34", 2)]
        [TestCase("~1|12&3", int.MaxValue)]
        [TestCase(".*&a", 1)]
        [TestCase("a{3,4}&c{1,2}", 0)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMaxExpansionLength_WhenCalledForPattern(string pattern, int expectedMaxExpansionLength)
        {
            RegularExpressionFactory.InitFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            var factory = RegularExpressionFactory.GetFactoryAsSingleton();

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
        [TestCase("~1&12&3", 0)]
        [TestCase("~1|12&3", 0)]
        [TestCase(".*&a", 1)]
        [TestCase(".+&ab", 2)]
        [TestCase("a{3,4}&c{1,2}", 0)]
        public void TryLoadRegex_ReturnedExpressionYieldsExpectedMinExpansionLength_WhenCalledForPattern(string pattern, int expectedMinExpansionLength)
        {
            RegularExpressionFactory.InitFactory(RegexAlphabetFactory.Default(), Substitute.For<IRandomGenerator>(), 1);
            var factory = RegularExpressionFactory.GetFactoryAsSingleton();

            factory.TryGetRegularExpression(pattern, out var union);
            var result = union.MaxExpansionRange.Start;
            
            Assert.AreEqual(expectedMinExpansionLength, result);
        }
        
        [Test]
        public void GetFullCharacterClassExpression_ReturnsCharacterClassContainingFullAlphabet()
        {
            var alphabet = Substitute.For<IParserAlphabet>();
            alphabet.GetAllCharacters().Returns(new List<string> {"a", "1"});
            RegularExpressionFactory.InitFactory(alphabet, Substitute.For<IRandomGenerator>(), 1);
            var factory = RegularExpressionFactory.GetFactoryAsSingleton();
            
            var result = factory.GetFullCharacterClassExpression();
            
            Assert.IsInstanceOf<CharacterClassExpression>(result);
            Assert.AreEqual(2, result.GetCharacterCount());
        }
    }
}