using NUnit.Framework;

namespace Regseed.Test
{
    [TestFixture]
    public class RegSeedExtensionTest
    {
        [TestCase("\\w")]
        [TestCase("\\W")]
        [TestCase("\\d")]
        [TestCase("\\D")]
        [TestCase("\\s")]
        [TestCase("\\S")]
        public void ReplaceRegexWildCard_ReturnsStringWithWildCardCorrectlyReplaced_WhenStringContainsWildCard(string wildCard)
        {
            var pattern = $"wildcard: {wildCard}";

            var result = pattern.ReplaceRegexWildCards();

            Assert.IsFalse(result.Contains(wildCard));
        }

        [Test]
        public void ReplaceRegexWildCard_DoesNotReplaceWildCard_WhenWildCardIsUnknown()
        {
            const string pattern = "wildcard: \\x";

            var result = pattern.ReplaceRegexWildCards();

            Assert.IsTrue(result.Contains("\\x"));
        }


        [Test]
        public void ReplaceRegexWildCard_DoesNotThrow_WhenStringDoesNotContainWildCard()
        {
            const string pattern = "no wildcard";

            Assert.DoesNotThrow(() => pattern.ReplaceRegexWildCards());
        }

        [Test]
        public void ReplaceRegexWildCards_LeavesOriginalPatternUnaltered()
        {
            var pattern = "\\w";

            pattern.ReplaceRegexWildCards();

            Assert.AreEqual("\\w", pattern);
        }

        [Test]
        public void ReplaceRegexWildCards_ReturnsNull_WhenOriginalIsNull()
        {
            string pattern = null;

            var result = pattern.ReplaceRegexWildCards();

            Assert.IsNull(result);
        }
    }
}