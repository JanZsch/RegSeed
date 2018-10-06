using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Results;
using Regseed.Parser.RegexTokens;
using Regseed.Parser.TokenParser;
using Regseed.Streams;

namespace Regseed.Test.Parser.TokenParser
{
    [TestFixture]
    public class BaseTokenParserTest : BaseTokenParser
    {
        [SetUp]
        public void Setup()
        {
            _initialStreamPosition = 0;
            _tryGetTokenWithoutNullCheckCalls = 0;
        }

        private int _tryGetTokenWithoutNullCheckCalls;

        protected override IParseResult TryGetTokenWithoutNullCheck(IStringStream inputStream, out IToken token)
        {
            _tryGetTokenWithoutNullCheckCalls++;
            token = null;
            return new SuccessParseResult();
        }

        [Test]
        public void TryGetToken_CallsTryGetTokenWithoutNullCheckOnce()
        {
            TryGetToken(Substitute.For<IStringStream>(), out _);

            Assert.AreEqual(1, _tryGetTokenWithoutNullCheckCalls);
        }

        [Test]
        public void TryGetToken_ReturnsFailureResult_WhenStreamIsNull()
        {
            var result = TryGetToken(null, out _);

            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void TryGetToken_SetsInitialStreamPositionTo3_WhenCurrentStreamPositionIs3()
        {
            var stream = Substitute.For<IStringStream>();
            stream.CurrentPosition.Returns(3);

            TryGetToken(stream, out _);

            Assert.AreEqual(3, _initialStreamPosition);
        }
    }
}