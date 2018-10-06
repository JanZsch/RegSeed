using System;
using NUnit.Framework;
using Regseed.Parser.RegexTokens;

namespace Regseed.Test.Parser.RegexTokens
{
    [TestFixture]
    public class CharacterTokenTest
    {
        private enum TestColor
        {
        }

        [Test]
        public void GetType_DoesNotThrow_WhenEnumTypeIsRegexTokenType()
        {
            var token = new CharacterToken(null, 0, 0);
            Assert.DoesNotThrow(() => token.GetType<RegexTokenType>());
        }

        [Test]
        public void GetType_ThrowsTypeAccessException_WhenEnumTypeNotRegexTokenType()
        {
            var token = new CharacterToken(null, 0, 0);
            Assert.Throws<TypeAccessException>(() => token.GetType<TestColor>());
        }
    }
}