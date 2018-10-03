using NUnit.Framework;
using Regseed.RegexSeeder.RegexTokens;

namespace Regseed.Test.RegexSeeder.RegexTokens.TokenParser
{
    [TestFixture]
    public class RegexSingleCharacterBaseTokenTest : RegexSingleCharacterBaseToken
    {
        public RegexSingleCharacterBaseTokenTest() : base(0)
        {
        }
        
        public RegexSingleCharacterBaseTokenTest(long position) : base(position)
        {
        }

        [Test]
        public void Length_Returns1()
        {
            var token = new RegexSingleCharacterBaseTokenTest(12);
            
            Assert.AreEqual(1, token.Length);
        }
    }
}