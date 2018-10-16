using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;
using Regseed.Factories;
using Regseed.Parser;

namespace Regseed.Test.Factories
{
    [TestFixture]
    public class StringFactoryTest
    {
        [SetUp]
        public void SetUp()
        {
            _random = Substitute.For<IRandomGenerator>();
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);

            _charClass1 = new CharacterClassExpression(_alphabet, _random);
            _charClass1.TryAddCharacters(new List<string> {"a", "b", "c"});
            _charClass2 = new CharacterClassExpression(_alphabet, _random);
            _charClass2.TryAddCharacters(new List<string> {"b", "c", "d"});
        }

        private IRandomGenerator _random;
        private IParserAlphabet _alphabet;

        private CharacterClassExpression _charClass1;
        private CharacterClassExpression _charClass2;

        [Test]
        public void ConcatWith_ReturnsFactoryGeneratingLength2Strings_WhenEachFactoryGeneratesLength1Strings()
        {
            var factory1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1});
            var factory2 = new StringBuilder(new List<CharacterClassExpression> {_charClass2});

            var result = factory1.ConcatWith(factory2).GenerateString();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("ab", result);
        }

        [Test]
        public void GenerateString_ReturnsTwoAA_WhenFactoryContainsTwoCharacterClassesWithSingleCharacterA()
        {
            var charClass = new CharacterClassExpression(_alphabet, _random);
            charClass.TryAddCharacters(new List<string> {"a"});

            var factory = new StringBuilder(new List<CharacterClassExpression> {charClass, charClass});

            var result = factory.GenerateString();

            Assert.AreEqual("aa", result);
        }

        [Test]
        public void GenerateString_ReturnsTwoLetterWord_WhenFactoryContainsTwoCharacterClasses()
        {
            var factory = new StringBuilder(new List<CharacterClassExpression> {_charClass1, _charClass2});

            var result = factory.GenerateString();

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void IntersectWith_ReturnsLetterC_WhenIntersectionIsBCAndRandomReturns1()
        {
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var factory1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1});
            var factory2 = new StringBuilder(new List<CharacterClassExpression> {_charClass2});

            var resultFactory = factory1.IntersectWith(factory2);
            var result = resultFactory.GenerateString();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("c", result);
        }

        [Test]
        public void Inverse_ReturnsWordDD_WhenContains2CharacterClassWithLettersABCAndAlphabetLettersABCD()
        {
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
            _alphabet.GetAllCharacters().Returns(new List<string> {"a", "b", "c", "d"});
            var factory1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1, _charClass1});

            var result = factory1.Inverse().GenerateString();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("dd", result);
        }

        [Test]
        public void MergeWith_ReturnsLetterD_WhenMergeIsABCDAndRandomReturns3()
        {
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(3);
            var factory1 = new StringBuilder(new List<CharacterClassExpression> {_charClass1});
            var factory2 = new StringBuilder(new List<CharacterClassExpression> {_charClass2});

            var result = factory1.MergeWith(factory2).GenerateString();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("d", result);
        }
    }
}