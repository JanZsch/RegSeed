using NUnit.Framework;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class EmptyExpressionTest
    {
        [Test]
        public void GetComplement_ReturnsOriginialEmptyExpression()
        {
            var expression = new EmptyExpression();

            var result = expression.GetComplement();

            Assert.AreEqual(expression, result);
        }

        [Test]
        public void ToRegexString_ReturnsEmptyString()
        {
            var expression = new EmptyExpression();

            var result = expression.ToRegexString();

            Assert.AreEqual(string.Empty, result);
        }
    }
}