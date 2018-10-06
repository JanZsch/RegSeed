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
    internal class CharacterClassExpressionTest : CharacterClassExpression
    {
        [SetUp]
        public void Setup()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _randomGenerator = Substitute.For<IRandomGenerator>();

            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
        }

        private IParserAlphabet _alphabet;
        private IRandomGenerator _randomGenerator;

        public CharacterClassExpressionTest()
        {
        }

        public CharacterClassExpressionTest(IParserAlphabet alphabet, IRandomGenerator random) : base(alphabet, random)
        {
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenAlphabetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CharacterClassExpression(null, _randomGenerator));
        }

        [Test]
        public void GetComplement_ReturnsF_WhenCharacterClassContainsAllLettersButF()
        {
            _randomGenerator = new RandomGenerator(new Random());
            _alphabet.GetAllCharacters().Returns(new List<string> {"J", "F", "r", "a", "n"});
            var expression = new CharacterClassExpression(_alphabet, _randomGenerator);
            expression.TryAddCharacters(new List<string> {"J", "r", "a", "n"});

            var complement = expression.GetComplement();
            var result = complement.ToRegexString();

            Assert.AreEqual("F", result);
        }

        [Test]
        public void ToSingleRegexString_DoesNotCallRandomGenerator_WhenCharacterClassEmpty()
        {
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);

            var result = expression.ToSingleRegexString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsEmptyString_WhenCharacterClassContainsOneCharacter()
        {
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);
            expression.TryAddCharacters(new List<string> {"U"});

            var result = expression.ToSingleRegexString();

            Assert.AreEqual("U", result);
            _randomGenerator.Received(0).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void ToSingleRegexString_ReturnsL_WhenRandomGeneratorReturns2AndSecondLetterInListIsL()
        {
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);
            expression.TryAddCharacters(new List<string> {"U", "L"});

            var result = expression.ToSingleRegexString();

            Assert.AreEqual("L", result);
            _randomGenerator.Received(1).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void TryAddCharacters_ReturnsFailureResult_WhenCharacterListContainsInvalidLetter()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);
            var letters = new List<string> {"s"};

            var result = new CharacterClassExpression(_alphabet, _randomGenerator).TryAddCharacters(letters);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryAddCharacters_ReturnsFailureResult_WhenCharacterListIsNull()
        {
            var result = new CharacterClassExpression(_alphabet, _randomGenerator).TryAddCharacters(null);

            Assert.IsFalse(result);
        }
    }
}