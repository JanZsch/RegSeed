using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Parser;
using Regseed.Common.Ranges;

namespace Regseed.Test.Common.Parser.Literals
{
    [TestFixture]
    public class CharacterRangeTest
    {
        private IParserAlphabet _alphabet;
        
        [SetUp]
        public void Setup()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
        }

        [TestCase(null)]
        [TestCase("")]
        public void Constructor_ThrowsArgumentNullException_WhenStartNullOrEmpty(string start)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CharacterRange(_alphabet, start, "s"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_ThrowsArgumentNullException_WhenEndNullOrEmpty(string end)
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CharacterRange(_alphabet, "s", end));
        }
        
        [Test]
        public void Constructor_DoesNotThrow_WhenStartAndEndNotNull()
        {
            Assert.DoesNotThrow(() => _ = new CharacterRange(_alphabet, "f", "r"));
        }
        
        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenCharacterListIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CharacterRange(null));
        }
        
        [Test]
        public void Constructor_DoesNotThrows_WhenCharacterListIsNotNull()
        {
            Assert.DoesNotThrow(() => _ = new CharacterRange(new List<string>()));
        }
    }
}