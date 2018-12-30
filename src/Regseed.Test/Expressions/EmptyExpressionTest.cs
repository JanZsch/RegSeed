using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;
using Regseed.Parser;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class EmptyExpressionTest
    {
        [Test]
        public void GetInverse_ReturnsCharClassExpressionWithRepeatRangeBetween1andMaxInverseLength()
        {
            const int maxInverseLength = 3;
            RegularExpressionFactory.InitFactory(Substitute.For<IParserAlphabet>(), Substitute.For<IRandomGenerator>(), maxInverseLength);
            var expression = new EmptyExpression();

            var result = expression.GetInverse();

            Assert.IsInstanceOf<CharacterClassExpression>(result);
            Assert.AreEqual(1, result.RepeatRange.Start);
            Assert.AreEqual(maxInverseLength, result.RepeatRange.End);
        }

        [Test]
        public void ToStringBuilder_ReturnsEmptyString()
        {
            var expression = new EmptyExpression();

            var result = expression.ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void Clone_ReturnsEmptyExpression()
        {
            var expression = new EmptyExpression();

            var result = expression.Clone();

            Assert.IsInstanceOf<EmptyExpression>(result);
        }
    }
}