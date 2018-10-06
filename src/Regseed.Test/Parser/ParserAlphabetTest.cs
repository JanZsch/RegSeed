using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Parser;
using Regseed.Parser.TokenParser;

namespace Regseed.Test.Parser
{
    [TestFixture]
    public class ParserAlphabetTest
    {
        [TestCase("e", "f")]
        [TestCase("f", "e")]
        [TestCase(null, "e")]
        [TestCase("e", null)]
        public void TryGetRange_ReturnsFailureResult_WhenOneOfTheArgumentsWasNotAdded(string start, string end)
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            var result = parser.TryGetRange(start, end, out _);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestCase("a")]
        [TestCase("b")]
        [TestCase("c")]
        public void GetRange_ReturnsListWith1Element_WhenStartAndEndAreEqual(string expectedLetter)
        {
            var parser = GetParser("a", "b", "c");

            var parseResult = parser.TryGetRange(expectedLetter, expectedLetter, out var result);

            Assert.IsTrue(parseResult.IsSuccess);
            Assert.AreEqual(expectedLetter, result.FirstOrDefault());
        }

        private static ParserAlphabet GetParser(params string[] letters)
        {
            var parser = new ParserAlphabet();

            foreach (var letter in letters)
                parser.Add(letter, Substitute.For<ITokenParser>());

            return parser;
        }

        [Test]
        public void Add_ReturnsAlphabet_WhenLetterSuccessfullyAdded()
        {
            var parser = new ParserAlphabet();

            var result = parser.Add("e", Substitute.For<ITokenParser>());

            Assert.AreEqual(parser, result);
        }

        [Test]
        public void Add_ThrowsException_WhenLetterAlreadyRegistered()
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            Assert.Throws<ArgumentException>(() => parser.Add("e", Substitute.For<ITokenParser>()));
        }

        [Test]
        public void Add_ThrowsException_WhenLetterIsNull()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentNullException>(() => parser.Add(null, Substitute.For<ITokenParser>()));
        }

        [Test]
        public void Add_ThrowsException_WhenLetterWithMoreThanOneCharacterIsAdded()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentException>(() => parser.Add("ex", Substitute.For<ITokenParser>()));
        }

        [Test]
        public void Add_ThrowsException_WhenTokenParserIsNull()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentNullException>(() => parser.Add("e", null));
        }

        [Test]
        public void GetRange_ReturnedListContainsAllElementsBetweenStartAndEnd_WhenStartAndEndAreNotEqual()
        {
            var parser = GetParser("a", "b", "c", "d", "e");

            parser.TryGetRange("b", "d", out var result);

            Assert.AreEqual("b", result[0]);
            Assert.AreEqual("c", result[1]);
            Assert.AreEqual("d", result[2]);
        }

        [Test]
        public void GetRange_ReturnsFailureResult_WhenStartPositionIsLargerThanEndPositionWithRespectToAddOrder()
        {
            var parser = GetParser("a", "b", "c");

            var result = parser.TryGetRange("c", "a", out _);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void IsValid_ReturnsFalse_WhenLetterWasNotAdded()
        {
            var parser = new ParserAlphabet();

            var result = parser.IsValid("e");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_ReturnsTrue_WhenLetterWasAdded()
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            var result = parser.IsValid("e");

            Assert.IsTrue(result);
        }

        [Test]
        public void RemoveCharacter_DoesNotThrow_WhenNonAlphabetCharacterIsRemoved()
        {
            var alphabet = new ParserAlphabet();
            alphabet.Add("j", Substitute.For<ITokenParser>()).Add("a", Substitute.For<ITokenParser>());

            Assert.DoesNotThrow(() => alphabet.RemoveCharacter("n"));
        }

        [Test]
        public void RemoveCharacter_RemovedCharacterNotValid_WhenWasCharacterWasValidAndAddedBefore()
        {
            var alphabet = new ParserAlphabet();
            alphabet.Add("F", Substitute.For<ITokenParser>()).Add("r", Substitute.For<ITokenParser>());

            var isValidResult = alphabet.IsValid("F");
            Assert.IsTrue(isValidResult);

            alphabet.RemoveCharacter("F");

            var isInvalidResult = alphabet.IsValid("F");
            Assert.IsFalse(isInvalidResult);
        }

        [Test]
        public void TryGetTokenParser_ReturnsFalse_WhenInitiallyValidCharacterIsRemoved()
        {
            var alphabet = new ParserAlphabet();
            alphabet.Add("F", Substitute.For<ITokenParser>()).Add("r", Substitute.For<ITokenParser>());

            var getParserResult = alphabet.TryGetTokenParser("F", out _);
            Assert.IsTrue(getParserResult);

            alphabet.RemoveCharacter("F");

            getParserResult = alphabet.TryGetTokenParser("F", out _);
            Assert.IsFalse(getParserResult);
        }

        [Test]
        public void TryGetTokenParser_ReturnsFalse_WhenLetterNotRegistered()
        {
            var parser = new ParserAlphabet();

            var result = parser.TryGetTokenParser("e", out _);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryGetTokenParser_ReturnsTrue_WhenLetterRegistered()
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            var result = parser.TryGetTokenParser("e", out var tokenParser);

            Assert.IsTrue(result);
            Assert.IsNotNull(tokenParser);
        }
    }
}