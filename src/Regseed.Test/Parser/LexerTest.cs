using System;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Results;
using Regseed.Parser;
using Regseed.Parser.RegexTokens;
using Regseed.Parser.TokenParser;
using Regseed.Streams;

namespace Regseed.Test.Parser
{
    [TestFixture]
    public class LexerTest
    {
        [SetUp]
        public void Setup()
        {
            _isEmptyCalls = 0;
            _tokenParser = Substitute.For<ITokenParser>();
            _input = Substitute.For<IStringStream>();
            _alphabet = Substitute.For<IParserAlphabet>();
        }

        private int _isEmptyCalls;
        private ITokenParser _tokenParser;
        private IParserAlphabet _alphabet;
        private IStringStream _input;

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenParserAlphabetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new Lexer(null));
        }

        [Test]
        public void TryConvertToTokenStream_ReturnsFailureResult_WhenInputStreamContainsKnownButInvalidLetter()
        {
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(true);
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);
            _input.IsEmpty().Returns(false);
            var lexer = new Lexer(_alphabet);

            var result = lexer.TryConvertToTokenStream(_input, out _);

            Assert.IsFalse(result.IsSuccess);
            _input.Received(1).IsEmpty();
            _alphabet.Received(1).TryGetTokenParser(Arg.Any<string>(), out _);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
        }

        [Test]
        public void TryConvertToTokenStream_ReturnsFailureResult_WhenInputStreamContainsUnknownLetter()
        {
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(false);
            _input.IsEmpty().Returns(false);
            var lexer = new Lexer(_alphabet);

            var result = lexer.TryConvertToTokenStream(_input, out _);

            Assert.IsFalse(result.IsSuccess);
            _input.Received(1).IsEmpty();
            _alphabet.Received(1).TryGetTokenParser(Arg.Any<string>(), out _);
        }

        [Test]
        public void TryConvertToTokenStream_ReturnsParseTreeWithOneElement_WhenInputContainsOneValidLetter()
        {
            _input.IsEmpty().Returns(x => _isEmptyCalls++ != 0);
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = _tokenParser;
                return true;
            });
            _tokenParser.TryGetToken(Arg.Any<IStringStream>(), out _).Returns(x =>
            {
                x[1] = Substitute.For<IToken>();
                return new SuccessParseResult();
            });
            var lexer = new Lexer(_alphabet);

            var parseResult = lexer.TryConvertToTokenStream(_input, out var result);

            Assert.IsTrue(parseResult.IsSuccess);
            Assert.IsFalse(result.IsEmpty());
            result.Pop();
            Assert.IsTrue(result.IsEmpty());
            _input.Received(2).IsEmpty();
            _tokenParser.Received(1).TryGetToken(Arg.Any<IStringStream>(), out _);
        }


        [Test]
        public void TryConvertToTokenStream_ThrowsArgumentException_WhenTokenParserFailsToParseToken()
        {
            _input.IsEmpty().Returns(false);
            _input.LookAhead(Arg.Any<long>()).Returns(string.Empty);
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = _tokenParser;
                return true;
            });
            _tokenParser.TryGetToken(Arg.Any<IStringStream>(), out _).Returns(new FailureParseResult(0));

            var lexer = new Lexer(_alphabet);

            var result = lexer.TryConvertToTokenStream(_input, out _);

            Assert.IsFalse(result.IsSuccess);
            _tokenParser.Received(1).TryGetToken(Arg.Any<IStringStream>(), out _);
        }

        [Test]
        public void TryConvertToTokenStream_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var lexer = new Lexer(_alphabet);

            Assert.Throws<ArgumentNullException>(() => lexer.TryConvertToTokenStream(null, out _));
        }
    }
}