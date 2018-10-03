using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Parser;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
using Regseed.Common.Streams;
using Regseed.RegexSeeder.RegexTokens.TokenParser;

namespace Regseed.Test.RegexSeeder.RegexTokens.TokenParser
{
    [TestFixture]
    public class CharacterParserTest
    {
        private IPrimitiveParser _primitiveParser;
        private IStringStream _stream;
        private IParseResult<CharacterRange> _classRangeParserFailure;
        private IParseResult<string> _stringParserFailure;
        private IParseResult<CharacterRange> _classRangeParserSuccess;
        private IParseResult<string> _stringParserSuccess;
        
        
        [SetUp]
        public void SetUp()
        {
            _classRangeParserFailure = Substitute.For<IParseResult<CharacterRange>>();
            _classRangeParserFailure.IsSuccess.Returns(false);
            _stringParserFailure = Substitute.For<IParseResult<string>>();
            _stringParserFailure.IsSuccess.Returns(false);
            _classRangeParserSuccess = Substitute.For<IParseResult<CharacterRange>>();
            _classRangeParserSuccess.IsSuccess.Returns(true);
            _stringParserSuccess = Substitute.For<IParseResult<string>>();
            _stringParserSuccess.IsSuccess.Returns(true);
            _stringParserSuccess.Value.Returns("F");
            
            _primitiveParser = Substitute.For<IPrimitiveParser>();
            _stream = Substitute.For<IStringStream>();

            _stream.CurrentPosition.Returns(2);
            _stream.Count.Returns(5);
        }

        [Test]
        public void TryGetToken_ReturnsFailureResult_WhenStringStreamIsNull()
        {
            var parser = new CharacterParser(_primitiveParser);

            var result = parser.TryGetToken(null, out _);

            Assert.IsInstanceOf<FailureParseResult>(result);
        }
                
        [Test]
        public void TryGetToken_ReturnsFailureResult_WhenInputIsNoCharacterOrClassRange()
        {
            _primitiveParser.TryParseCharacterRange(Arg.Any<IStringStream>()).Returns(_classRangeParserFailure);
            _primitiveParser.TryParseCharacter(Arg.Any<IStringStream>()).Returns(_stringParserFailure);
            var parser = new CharacterParser(_primitiveParser);

            var result = parser.TryGetToken(_stream, out _);

            Assert.IsFalse(result.IsSuccess);
            _primitiveParser.Received(1).TryParseCharacterRange(Arg.Any<IStringStream>());
            _primitiveParser.Received(1).TryParseCharacter(Arg.Any<IStringStream>());
        }
        
        [Test]
        public void TryGetToken_ReturnsSuccessResult_WhenInputIsClassRange()
        {
            _primitiveParser.TryParseCharacterRange(Arg.Any<IStringStream>()).Returns(_classRangeParserSuccess);
            var parser = new CharacterParser(_primitiveParser);

            var result = parser.TryGetToken(_stream, out _);

            Assert.IsTrue(result.IsSuccess);
            _primitiveParser.Received(1).TryParseCharacterRange(Arg.Any<IStringStream>());
            _primitiveParser.Received(0).TryParseCharacter(Arg.Any<IStringStream>());
        }
        
        [Test]
        public void TryGetToken_ReturnsSuccessResult_WhenInputIsSingleCharacter()
        {
            _primitiveParser.TryParseCharacterRange(Arg.Any<IStringStream>()).Returns(_classRangeParserFailure);
            _primitiveParser.TryParseCharacter(Arg.Any<IStringStream>()).Returns(_stringParserSuccess);
            var parser = new CharacterParser(_primitiveParser);

            var result = parser.TryGetToken(_stream, out _);

            Assert.IsTrue(result.IsSuccess);
            _primitiveParser.Received(1).TryParseCharacterRange(Arg.Any<IStringStream>());
            _primitiveParser.Received(1).TryParseCharacter(Arg.Any<IStringStream>());
        }
    }
}