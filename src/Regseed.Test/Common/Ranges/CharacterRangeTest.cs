using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Ranges;
using Regseed.Parser;

namespace Regseed.Test.Common.Ranges
{
    [TestFixture]
    public class CharacterRangeTest
    {
        [SetUp]
        public void Setup()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
        }

        private IParserAlphabet _alphabet;

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
        public void Constructor_DoesNotThrows_WhenCharacterListIsNotNull()
        {
            Assert.DoesNotThrow(() => _ = new CharacterRange(new List<string>()));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenCharacterListIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new CharacterRange(null));
        }
    }
}