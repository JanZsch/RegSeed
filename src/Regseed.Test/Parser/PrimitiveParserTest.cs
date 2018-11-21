using System;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Results;
using Regseed.Parser;
using Regseed.Parser.PrimitiveParsers;
using Regseed.Resources;
using Regseed.Streams;

namespace Regseed.Test.Parser
{
    [TestFixture]
    public class PrimitiveParserTest
    {
        [SetUp]
        public void SetUp()
        {
            _popCount = 0;
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
        }

        private int _popCount;
        private IParserAlphabet _alphabet;

        [TestCase("U")]
        [TestCase("Ulrike")]
        public void TryParseCharacter_ReturnsSuccessResult_WhenStringContainsValidCharacters(string input)
        {
            var stream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("U", result.Value);
        }

        [TestCase("\\F", "F")]
        [TestCase("\\Franziska", "F")]
        [TestCase("\\\\", "\\")]
        public void TryParseCharacter_ReturnsSuccessResult_WhenStringStartsWithEscapeAndContainsOnlyValidCharacters(
            string input, string expected)
        {
            var stream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expected, result.Value);
            stream.Received(2).Pop();
        }

        [TestCase("-")]
        [TestCase("a")]
        [TestCase("ab")]
        public void TryParseInteger_ReturnsFailureResult_WhenInputStreamIsFaulty(string inputString)
        {
            var stream = GetStringStreamFor(inputString);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseInteger(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.IntegerExpected, result.ErrorType);
            stream.Received(0).Pop();
        }

        [TestCase("10212", 10212)]
        [TestCase("89Franziska", 89)]
        [TestCase("0", 0)]
        [TestCase("-0", 0)]
        [TestCase("-850", -850)]
        [TestCase("-8.50", -8)]
        [TestCase("8-50", 8)]
        public void TryParseInteger_ReturnsSuccessResult_WhenStringStartsWithInteger(string input, int expected)
        {
            var stream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseInteger(stream);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expected, result.Value);
        }

        [TestCase("")]
        [TestCase("a")]
        [TestCase("ab")]
        public void TryParseCharacterRange_ReturnsFailureResult_WhenInputStreamIsTooShortForCharacterRange(string input)
        {
            var stream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacterRange(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.None, result.ErrorType);
            stream.Received(0).Pop();
        }

        [TestCase("abs")]
        [TestCase("\\abs")]
        public void TryParseCharacterRange_ReturnsFailureResult_WhenSecondCharacterIsNotRangeSeparator(
            string inputStream)
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            var stream = GetStringStreamFor(inputStream);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacterRange(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.None, result.ErrorType);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
            stream.Received(0).Pop();
        }

        [TestCase("a-s")]
        [TestCase("\\a-s")]
        public void TryParseCharacterRange_ReturnsFailureResult_WhenThirdCharacterIsInvalidCharacter(string inputStream)
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(x => !x[0].Equals("s"));
            var stream = GetStringStreamFor(inputStream);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacterRange(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.CharacterRangeExpected, result.ErrorType);
            _alphabet.Received(2).IsValid(Arg.Any<string>());
            stream.Received(0).Pop();
        }

        [TestCase("a-d", "a", "d", 3)]
        [TestCase("\\--\\-", "-", "-", 5)]
        [TestCase("?-9", "?", "9", 3)]
        [TestCase("\\a-\\s", "a", "s", 5)]
        [TestCase("j-an", "j", "a", 3)]
        public void TryParseCharacterRange_ReturnsSuccessResult_WhenStreamIsCorrectCharacterRange(string inputStream,
            string expectedStart, string expectedEnd, int expectedPops)
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.TryGetRange(Arg.Any<string>(), Arg.Any<string>(), out _).Returns(new SuccessResult());
            var stream = GetStringStreamFor(inputStream);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacterRange(stream);

            Assert.IsTrue(result.IsSuccess);
            _alphabet.Received(2).IsValid(Arg.Any<string>());
            _alphabet.Received(1).TryGetRange(expectedStart, expectedEnd, out _);
            stream.Received(expectedPops).Pop();
        }

        [TestCase("")]
        [TestCase("[")]
        [TestCase("{a")]
        [TestCase("{12")]
        [TestCase("{12x")]
        [TestCase("{12,")]
        [TestCase("{12,x")]
        [TestCase("{12,-")]
        [TestCase("{12,-12")]
        [TestCase("{12,-12x")]
        [TestCase("{12.2,19}")]
        [TestCase("{12,-12")]
        [TestCase("{}")]
        public void TryParseIntegerInterval_FailureResult_WhenInputStringIsNoValidInterval(string input)
        {
            var inputStream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseIntegerInterval(inputStream);

            Assert.IsFalse(result.IsSuccess);
            inputStream.Received(0).Pop();
        }

        [TestCase("{,}", null, null, 3)]
        [TestCase("{1,}", 1, null, 4)]
        [TestCase("{,3}", null, 3, 4)]
        [TestCase("{1,2}", 1, 2, 5)]
        [TestCase("{1}", 1, 1, 3)]
        public void TryParseIntegerInterval_SuccessResult_WhenInputStringIsValidInterval(string input, int? expectedLower, int? expectedUpper, int expectedPopCalls)
        {
            var inputStream = GetStringStreamFor(input);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseIntegerInterval(inputStream);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedLower, result.Value.Start);
            Assert.AreEqual(expectedUpper, result.Value.End);
            inputStream.Received(expectedPopCalls).Pop();
        }

        private IStringStream GetStringStreamFor(string streamString)
        {
            var stream = Substitute.For<IStringStream>();
            stream.Count.Returns(streamString.Length);
            stream.IsEmpty().Returns(x => _popCount >= streamString.Length);
            stream.Pop().Returns(x => streamString.Length == 0 ? "" : streamString[_popCount++].ToString());
            stream.LookAhead(Arg.Any<long>()).Returns(x =>
                streamString.Length == 0 ? "" : streamString[_popCount + (int) (long) x[0]].ToString());

            return stream;
        }

        [Test]
        public void Constructor_throwsArgumentNullException_WhenAlphabetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PrimitiveParser(null));
        }

        [Test]
        public void TryParseCharacter_ReturnsFailureResult_WhenCharacterNotValidCharacter()
        {
            var stream = GetStringStreamFor("a");
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);

            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsFalse(result.IsSuccess);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseCharacter_ReturnsFailureResult_WhenStringConsistsOfEscapeCharacterAndInvalidCharacter()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(x => !x[0].Equals("N"));
            var stream = GetStringStreamFor($"{SpecialCharacters.Escape}N");
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsFalse(result.IsSuccess);
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseCharacter_ReturnsFailureResult_WhenStringContainsOnlyEscapeCharacter()
        {
            var stream = GetStringStreamFor(SpecialCharacters.Escape);
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsFalse(result.IsSuccess);
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseCharacter_ReturnsFailureResult_WhenStringIsEmpty()
        {
            var stream = GetStringStreamFor("");
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacter(stream);

            Assert.IsFalse(result.IsSuccess);
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseCharacter_ThrowsArgumentNullException_WhenStreamIsNull()
        {
            var parser = new PrimitiveParser(_alphabet);

            Assert.Throws<ArgumentNullException>(() => parser.TryParseCharacter(null));
        }

        [Test]
        public void TryParseCharacterRange_ReturnsFailureResult_WhenFirstStreamCharacterIsNotValidCharacter()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);
            var stream = GetStringStreamFor("n-2");
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseCharacterRange(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.None, result.ErrorType);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseCharacterRange_ThrowsArgumentNullException_WhenInputStreamIsNull()
        {
            var parser = new PrimitiveParser(_alphabet);

            Assert.Throws<ArgumentNullException>(() => parser.TryParseCharacterRange(null));
        }

        [Test]
        public void TryParseInteger_ReturnsFailureResult_WhenInputStreamIsEmptyy()
        {
            var stream = GetStringStreamFor("");
            var parser = new PrimitiveParser(_alphabet);

            var result = parser.TryParseInteger(stream);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RegSeedErrorType.UnexpectedEndOfStream, result.ErrorType);
            stream.Received(0).Pop();
        }

        [Test]
        public void TryParseInteger_ThrowsArgumentNullException_WhenStreamNull()
        {
            var parser = new PrimitiveParser(_alphabet);

            Assert.Throws<ArgumentNullException>(() => parser.TryParseInteger(null));
        }

        [Test]
        public void TryParseIntegerInterval_ThrowsArgumentNullException_WhenStreamIsNll()
        {
            var parser = new PrimitiveParser(_alphabet);

            Assert.Throws<ArgumentNullException>(() => parser.TryParseIntegerInterval(null));
        }
    }
}