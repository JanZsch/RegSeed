using NSubstitute;
using NUnit.Framework;
using Regseed.Parser;
using Regseed.Parser.ParserFactories;
using Regseed.Parser.TokenParser;
using Regseed.Resources;

namespace Regseed.Test.Parser.ParserFactories
{
    [TestFixture]
    public class StatefulParserFactoryTest 
    {
        private IParserAlphabet _alphabet;

        [SetUp]
        public void SetUp()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
        }

        [Test]
        public void TryGetTokenParser_ReturnsFalse_WhenCharacterIsInvalid()
        {
            _alphabet.IsValid(Arg.Any<string>()).Returns(false);
            var factory = new StatefulParserFactory(_alphabet);

            var result = factory.TryGetTokenParser("s", out _);
            
            Assert.IsFalse(result);
        }
        
        [Test]
        public void TryGetTokenParser_ReturnsParserFromAlphabet_WhenCharacterClassStateIsNotActivated()
        {
            var parser = Substitute.For<ITokenParser>();
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = parser;
                return true;
            });

            var factory = new StatefulParserFactory(_alphabet);

            var result = factory.TryGetTokenParser("s", out var resultParser);
            
            Assert.IsTrue(result);
            Assert.AreEqual(parser, resultParser);
        }
        
        [Test]
        public void TryGetTokenParser_CharacterClassStateIsActivated_WhenOpenCharacterClassIsParsed()
        {
            var parser = Substitute.For<ITokenParser>();
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = parser;
                return true;
            });

            var factory = new StatefulParserFactory(_alphabet);
            factory.TryGetTokenParser("s", out var resultParser);            
            Assert.AreEqual(parser, resultParser);
            
            factory.TryGetTokenParser(SpecialCharacters.OpenCharacterClass, out resultParser);            
            Assert.AreEqual(parser, resultParser);

            factory.TryGetTokenParser("s", out resultParser);
            Assert.IsInstanceOf<CharacterClassCharacterParser>(resultParser);
        }
        
        [Test]
        public void TryGetTokenParser_CharacterClassStateIsDeactivated_WhenCloseCharacterClassIsParsed()
        {
            var parser = Substitute.For<ITokenParser>();
            _alphabet.TryGetTokenParser(Arg.Any<string>(), out _).Returns(x =>
            {
                x[1] = parser;
                return true;
            });

            var factory = new StatefulParserFactory(_alphabet);
            
            factory.TryGetTokenParser(SpecialCharacters.OpenCharacterClass, out var resultParser);            
            factory.TryGetTokenParser("s", out resultParser);
            Assert.IsInstanceOf<CharacterClassCharacterParser>(resultParser);
            
            factory.TryGetTokenParser(SpecialCharacters.CloseCharacterClass, out resultParser);
            Assert.AreEqual(parser, resultParser);
            
            factory.TryGetTokenParser("s", out resultParser);
            Assert.AreEqual(parser, resultParser);
        }
    }
}