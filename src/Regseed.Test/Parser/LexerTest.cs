using System;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Results;
using Regseed.Parser;
using Regseed.Parser.ParserFactories;
using Regseed.Parser.RegexTokens;
using Regseed.Parser.TokenParser;
using Regseed.Streams;

namespace Regseed.Test.Parser
{
    [TestFixture]
    public class LexerTest
    {
        private int _isEmptyCalls;
        private ITokenParser _tokenParser;
        private IParserFactory _parserFactory;
        private IStringStream _input;

        [SetUp]
        public void Setup()
        {
            _isEmptyCalls = 0;
            _tokenParser = Substitute.For<ITokenParser>();
            _input = Substitute.For<IStringStream>();
            _parserFactory = Substitute.For<IParserFactory>();
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenParserAlphabetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new Lexer(null));
        }

        [Test]
        public void TryConvertToTokenStream_ReturnsFailureResult_WhenInputStreamContainsInvalidCharacter()
        {
            _parserFactory.TryGetTokenParser(Arg.Any<string>(), out _).Returns(false);
            _input.IsEmpty().Returns(false);
            var lexer = new Lexer(_parserFactory);

            var result = lexer.TryConvertToTokenStream(_input, out _);

            Assert.IsFalse(result.IsSuccess);
            _input.Received(1).IsEmpty();
            _parserFactory.Received(1).TryGetTokenParser(Arg.Any<string>(), out _);
        }

        [Test]
        public void TryConvertToTokenStream_ReturnsParseTreeWithOneElement_WhenInputContainsOneValidLetter()
        {
            _input.IsEmpty().Returns(x => _isEmptyCalls++ != 0);
            _parserFactory.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = _tokenParser;
                return true;
            });
            _tokenParser.TryGetToken(Arg.Any<IStringStream>(), out _).Returns(x =>
            {
                x[1] = Substitute.For<IToken>();
                return new SuccessParseResult();
            });
            var lexer = new Lexer(_parserFactory);

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
            _parserFactory.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = _tokenParser;
                return true;
            });
            _tokenParser.TryGetToken(Arg.Any<IStringStream>(), out _).Returns(new FailureParseResult(0));

            var lexer = new Lexer(_parserFactory);

            var result = lexer.TryConvertToTokenStream(_input, out _);

            Assert.IsFalse(result.IsSuccess);
            _tokenParser.Received(1).TryGetToken(Arg.Any<IStringStream>(), out _);
        }

        [Test]
        public void TryConvertToTokenStream_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var lexer = new Lexer(_parserFactory);

            Assert.Throws<ArgumentNullException>(() => lexer.TryConvertToTokenStream(null, out _));
        }
    }
}