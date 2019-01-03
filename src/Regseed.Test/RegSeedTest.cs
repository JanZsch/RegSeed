using System;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.TokenParser;

namespace Regseed.Test
{
    [TestFixture]
    public class RegSeedTest
    {
        [Test]
        public void ConstructorWithoutPattern_GenerateThrowsException_WhenNoPatternWasLoaded()
        {
            var regseed = new RegSeed();

            Assert.Throws<ArgumentException>(() => regseed.Generate());
        }

        [Test]
        public void ConstructorWithPattern_GenerateCreatesString_WhenPatternWasLoaded()
        {
            var regseed = new RegSeed("a");

            var result = regseed.Generate();

            Assert.AreEqual("a", result);
        }

        [Test]
        public void ConstructorWithPattern_ThrowsArgumentException_WhenPatternInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var unused = new RegSeed("{");
            });
        }

        [Test]
        public void ConstructorWithPattern_GenerateCreatesString_WhenCalledWithRandomGenerator()
        {
            var regseed = new RegSeed("a", new Random());

            var result = regseed.Generate();

            Assert.AreEqual("a", result);
        }

        [Test]
        public void ConstructorWithPattern_GenerateCreatesString_WhenCalledWithParserAlphabet()
        {
            var alphabet = RegexAlphabetFactory.Minimal();
            var primitiveParser = new PrimitiveParser(alphabet);
            alphabet.Add("a", new CharacterParser(primitiveParser));
            var regseed = new RegSeed("a", new Random(), alphabet);

            var result = regseed.Generate();

            Assert.AreEqual("a", result);
        }

        [Test]
        public void ConstructorWithPattern_DoesNotReplaceWildCards_WhenGenerateIsCalled()
        {
            var regseed = new RegSeed("\\d");

            var result = regseed.Generate();

            Assert.AreEqual("d", result);
        }

        [Test]
        public void ConstructorWithPattern_UsesCustomRandomGenerator_WhenCustomRandomGeneratorProvided()
        {
            var random = Substitute.For<IRandomGenerator>();
            random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var regeseed = new RegSeed(random);
            regeseed.TryLoadRegexPattern("[a-z]");

            var result = regeseed.Generate();

            Assert.AreEqual("b", result);
        }

        [Test]
        public void ConstructorWithPattern_UsesCustomRandomGenerator_WhenCustomRandomGeneratorAndAlphabetProvidedProvided()
        {
            var alphabet = RegexAlphabetFactory.Minimal();
            var primitiveParser = new PrimitiveParser(alphabet);
            alphabet.Add("a", new CharacterParser(primitiveParser))
                .Add("b", new CharacterParser(primitiveParser));
            var random = Substitute.For<IRandomGenerator>();
            random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var regeseed = new RegSeed(random);
            regeseed.TryLoadRegexPattern("[a-b]");

            var result = regeseed.Generate();

            Assert.AreEqual("b", result);
        }

        [Test]
        public void EnableWildCards_ThrowsArgumentException_WhenCalledOnAlreadyInitialisedRegSeed()
        {
            var regeseed = new RegSeed();
            regeseed.TryLoadRegexPattern("a");

            Assert.Throws<ArgumentException>(() => regeseed.EnableStandardWildCards());
        }

        [Test]
        public void EnableWildCards_GenerateRespectsWildCard_WhenCalledBeforeInitialisation()
        {
            var regeseed = new RegSeed().EnableStandardWildCards();
            regeseed.TryLoadRegexPattern("\\d");

            var result = regeseed.Generate();

            Assert.AreNotEqual("d", result);
            Assert.AreEqual(1, result.Length);
            Assert.IsTrue(char.IsDigit(result.ToCharArray()[0]));
        }

        [Test]
        public void SetMaxCharInverseLength_GenerateReturnsStringsOfAtMostLength3_WhenCalledWith3()
        {
            const int runs = 200;
            var lengthCounts = new[] {0, 0, 0, 0};
            var regseed = new RegSeed().SetMaxCharClassInverseLength(3);
            regseed.TryLoadRegexPattern("~a");

            for (var i = 0; i < runs; i++)
                lengthCounts[regseed.Generate().Length]++;

            Assert.AreEqual(0, lengthCounts[0]);

            var totalCalls = 0;
            for (var i = 1; i <= 3; i++)
            {
                totalCalls += lengthCounts[i];
                Assert.Greater(lengthCounts[i], 0);
            }

            Assert.AreEqual(runs, totalCalls);
        }

        [Test]
        public void MaxCharInverseLength_Returns5_WhenSetMaxCharInverseLengthNotCalled()
        {
            var regseed = new RegSeed();

            var result = regseed.MaxCharClassInverseLength;

            Assert.AreEqual(5, result);
        }
    }
}