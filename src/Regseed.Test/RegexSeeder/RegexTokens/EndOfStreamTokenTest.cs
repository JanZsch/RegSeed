using System;
using NUnit.Framework;
using Regseed.RegexSeeder.RegexTokens;

namespace Regseed.Test.RegexSeeder.RegexTokens
{
    [TestFixture]
    public class EndOfStreamTokenTest
    {
        private enum TestColor
        { }
        
        [Test]
        public void GetType_ThrowsTypeAccessException_WhenEnumTypeNotRegexTokenType()
        {
            var token = new EndOfStreamToken(0);
            Assert.Throws<TypeAccessException>(() => token.GetType<TestColor>());
        }
        
        [Test]
        public void GetType_DoesNotThrow_WhenEnumTypeIsRegexTokenType()
        {
            var token = new EndOfStreamToken(0);
            Assert.DoesNotThrow(() => token.GetType<RegexTokenType>());
        }

    }
}