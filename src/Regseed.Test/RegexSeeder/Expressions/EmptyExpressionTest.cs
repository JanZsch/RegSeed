using NUnit.Framework;
using Regseed.RegexSeeder.Expressions;

namespace Regseed.Test.RegexSeeder.Expressions
{
    [TestFixture]
    public class EmptyExpressionTest
    {
        [Test]
        public void ToRegexString_ReturnsEmptyString()
        {
            var expression= new EmptyExpression();

            var result = expression.ToRegexString();
            
            Assert.AreEqual(string.Empty, result);
        }
        
        [Test]
        public void GetComplement_ReturnsOriginialEmptyExpression()
        {
            var expression= new EmptyExpression();

            var result = expression.GetComplement();
            
            Assert.AreEqual(expression, result);
        }
    }
}