using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Ranges;
using Regseed.Common.Results;
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
        public void TrySetRange_ReturnsFailureResult_WhenStartNullOrEmpty(string start)
        {
            var characterRange = new CharacterRange();
            
            var result = characterRange.TrySetRange(start, "s", _alphabet);
            
            Assert.IsFalse(result.IsSuccess);
        }

        [TestCase(null)]
        [TestCase("")]
        public void TrySetRange_ReturnsFailureResult_WhenEndNullOrEmpty(string end)
        {
            var characterRange = new CharacterRange();
            
            var result = characterRange.TrySetRange("s", end, _alphabet);
            
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void TrySetRange_ReturnsFailureResult_WhenAlphabetCannotRetrieveRange()
        {
            _alphabet.TryGetRange(Arg.Any<string>(), Arg.Any<string>(), out _).Returns(new FailureResult());
            var characterRange = new CharacterRange();
            
            var result = characterRange.TrySetRange("s", "a", _alphabet);
            
            Assert.IsFalse(result.IsSuccess);
            _alphabet.Received(1).TryGetRange(Arg.Any<string>(), Arg.Any<string>(), out _);
        }
    }
}