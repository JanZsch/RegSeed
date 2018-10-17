using NUnit.Framework;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class EmptyExpressionTest
    {
        [Test]
        public void GetComplement_ReturnsOriginalEmptyExpression()
        {
            var expression = new EmptyExpression();

            var result = expression.GetComplement();

            Assert.AreEqual(expression, result);
        }

        [Test]
        public void ToStringBuilder_ReturnsEmptyString()
        {
            var expression = new EmptyExpression();

            var result = expression.ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result);
        }
    }
}