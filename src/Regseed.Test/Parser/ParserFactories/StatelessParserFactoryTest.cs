using NSubstitute;
using NUnit.Framework;
using Regseed.Parser;
using Regseed.Parser.ParserFactories;
using Regseed.Parser.TokenParser;

namespace Regseed.Test.Parser.ParserFactories
{
    [TestFixture]
    public class StatelessParserFactoryTest
    {
        private IParserAlphabet _alphabet;

        [SetUp]
        public void SetUp()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
        }

        [Test]
        public void TryGetTokenParser_ReturnsFalse_WhenCharacterIsInvalid()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);
            var factory = new StatelessParserFactory(_alphabet);

            var result = factory.TryGetTokenParser("s", out _);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void TryGetTokenParser_ReturnsFalse_WhenCharacterIsValidButHasNoAssociatedParser()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(false);
            var factory = new StatelessParserFactory(_alphabet);

            var result = factory.TryGetTokenParser("s", out _);
            
            Assert.IsFalse(result);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
            _alphabet.Received(1).TryGetTokenParser(Arg.Any<string>(), out _);
        }
        
        [Test]
        public void TryGetTokenParser_ReturnsTrue_WhenCharacterIsValidAndHasAssociatedParser()
        {
            var parser = Substitute.For<ITokenParser>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = parser;
                return true;
            });
            var factory = new StatelessParserFactory(_alphabet);

            var result = factory.TryGetTokenParser("s", out var resultParser);
            
            Assert.IsTrue(result);
            Assert.AreEqual(parser, resultParser);
            _alphabet.Received(1).IsValid(Arg.Any<string>());
            _alphabet.Received(1).TryGetTokenParser(Arg.Any<string>(), out _);
        }
    }
}