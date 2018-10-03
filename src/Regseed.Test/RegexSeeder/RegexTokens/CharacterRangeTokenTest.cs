using System;
using NUnit.Framework;
using Regseed.RegexSeeder.RegexTokens;

namespace Regseed.Test.RegexSeeder.RegexTokens
{
    [TestFixture]
    public class CharacterRangeTokenTest
    {
        private enum TestColor
        { }
        
        [Test]
        public void GetType_ThrowsTypeAccessException_WhenEnumTypeNotRegexTokenType()
        {
            var token = new CharacterRangeToken(null, 0, 0);
            Assert.Throws<TypeAccessException>(() => token.GetType<TestColor>());
        }
        
        [Test]
        public void GetType_DoesNotThrow_WhenEnumTypeIsRegexTokenType()
        {
            var token = new CharacterToken(null, 0, 0);
            Assert.DoesNotThrow(() => token.GetType<RegexTokenType>());
        }

    }
}