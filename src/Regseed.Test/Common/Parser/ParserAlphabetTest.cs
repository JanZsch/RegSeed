using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Parser;
using Regseed.Common.Token;

namespace Regseed.Test.Common.Parser
{
    [TestFixture]
    public class ParserAlphabetTest
    {
        [Test]
        public void Add_ThrowsException_WhenLetterWithMoreThanOneCharacterIsAdded()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentException>(() => parser.Add("ex", Substitute.For<ITokenParser>()));
        }
        
        [Test]
        public void Add_ThrowsException_WhenLetterIsNull()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentNullException>(() => parser.Add(null, Substitute.For<ITokenParser>()));
        }
        
        [Test]
        public void Add_ThrowsException_WhenTokenParserIsNull()
        {
            var parser = new ParserAlphabet();

            Assert.Throws<ArgumentNullException>(() => parser.Add("e", null));
        }
        
        [Test]
        public void Add_ThrowsException_WhenLetterAlreadyRegistered()
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            Assert.Throws<ArgumentException>(() => parser.Add("e", Substitute.For<ITokenParser>()));
        }
        
        [Test]
        public void Add_ReturnsAlphabet_WhenLetterSuccessfullyAdded()
        {
            var parser = new ParserAlphabet();
            
            var result = parser.Add("e", Substitute.For<ITokenParser>());

            Assert.AreEqual(parser, result);
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

        [TestCase("e", "f")]
        [TestCase("f", "e")]
        [TestCase(null, "e")]
        [TestCase("e", null)]
        public void GetRange_ThrowsArgumentException_WhenOneOfTheArgumentsWasNotAdded(string start, string end)
        {
            var parser = new ParserAlphabet();
            parser.Add("e", Substitute.For<ITokenParser>());

            Assert.Throws<ArgumentException>(() => parser.GetRange(start, end));
        }
        
        [Test]
        public void GetRange_ThrowsArgumentException_WhenStartPositionIsLargerThanEndPositionWithRespectToAddOrder()
        {
            var parser = GetParser("a", "b", "c");

            Assert.Throws<ArgumentException>(() => parser.GetRange("c", "a"));
        }
        
        [TestCase("a")]
        [TestCase("b")]
        [TestCase("c")]
        public void GetRange_ReturnsListWith1Element_WhenStartAndEndAreEqual(string expectedLetter)
        {
            var parser = GetParser("a", "b", "c");


            var result = parser.GetRange(expectedLetter, expectedLetter);
            
            Assert.IsNotEmpty(result);
            Assert.AreEqual(expectedLetter, result.FirstOrDefault());
        }

        [Test]
        public void GetRange_ReturnedListContainsAllElementsBetweenStartAndEnd_WhenStartAndEndAreNotEqual()
        {
            var parser = GetParser("a", "b", "c", "d", "e");

            var result = parser.GetRange("b", "d");
            
            Assert.AreEqual("b", result[0]);
            Assert.AreEqual("c", result[1]);
            Assert.AreEqual("d", result[2]);
        }
        
        private static ParserAlphabet GetParser(params string[] letters)
        {
            var parser = new ParserAlphabet();

            foreach (var letter in letters)
                parser.Add(letter, Substitute.For<ITokenParser>());

            return parser;
        }

    }
}