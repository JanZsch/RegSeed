using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Expressions;
using Regseed.Parser;
using Regseed.Test.Mocks;

namespace Regseed.Test.Common.Builder
{
    [TestFixture]
    public class StringBuilderTest
    {
        [SetUp]
        public void SetUp()
        {
            _random = Substitute.For<IRandomGenerator>();
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);

            _charClass1 = new CharacterClassExpression(_alphabet, _random, 1);
            _charClass1.AddCharacters(new List<string> {"a", "b", "c"});
            _charClass2 = new CharacterClassExpression(_alphabet, _random, 1);
            _charClass2.AddCharacters(new List<string> {"b", "c", "d"});
        }

        private IRandomGenerator _random;
        private IParserAlphabet _alphabet;

        private CharacterClassExpression _charClass1;
        private CharacterClassExpression _charClass2;

        [Test]
        public void ConcatWith_ReturnsBuilderGeneratingLength2Strings_WhenEachbuilderGeneratesLength1Strings()
        {
            var builder1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1});
            var builder2 = new StringBuilderMock(new List<CharacterClassExpression> {_charClass2});

            var result = builder1.ConcatWith(builder2).GenerateString();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("ab", result);
        }

        [Test]
        public void GenerateString_ReturnsTwoAA_WhenBuilderContainsTwoCharacterClassesWithSingleCharacterA()
        {
            var charClass = new CharacterClassExpression(_alphabet, _random, 1);
            charClass.AddCharacters(new List<string> {"a"});

            var builder = new StringBuilder(new List<CharacterClassExpression> {charClass, charClass});

            var result = builder.GenerateString();

            Assert.AreEqual("aa", result);
        }

        [Test]
        public void GenerateString_ReturnsTwoLetterWord_WhenBuilderContainsTwoCharacterClasses()
        {
            var builder = new StringBuilder(new List<CharacterClassExpression> {_charClass1, _charClass2});

            var result = builder.GenerateString();

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void IntersectWith_ReturnsLetterC_WhenIntersectionIsBCAndRandomReturns1()
        {
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var builder1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1});
            var builder2 = new StringBuilderMock(new List<CharacterClassExpression> {_charClass2});

            var stringBuilder = builder1.IntersectWith(builder2);
            var result = stringBuilder.GenerateString();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("c", result);
        }
    }
}