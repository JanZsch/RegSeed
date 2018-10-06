using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class ConcatenationExpressionTest : ConcatenationExpression
    {
        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _expression = Substitute.For<IExpression>();
        }

        private IRandomGenerator _randomGenerator;
        private IExpression _expression;

        public ConcatenationExpressionTest() : base(null)
        {
        }

        public ConcatenationExpressionTest(IRandomGenerator random) : base(random)
        {
        }

        [Test]
        public void GetComplement_ResultHasSameRepeatIntervalAsOriginal()
        {
            var repeatInterval = new IntegerInterval(null, null);
            var concatExpression = new ConcatenationExpression(_randomGenerator) {RepeatRange = repeatInterval};

            var result = concatExpression.GetComplement();

            Assert.AreEqual(repeatInterval, result.RepeatRange);
        }

        [Test]
        public void GetComplement_ReturnValueContainsTwoExpressions_WhenExpresionContainsTwoElements()
        {
            var secondExpression = Substitute.For<IExpression>();
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression).Append(secondExpression);

            concatExpression.GetComplement();

            secondExpression.Received(1).GetComplement();
            _expression.Received(1).GetComplement();
        }

        [Test]
        public void ToSingleRegexString_ReturnsEmptyString_WhenConcatContainsNoElements()
        {
            var expression = new ConcatenationExpressionTest(_randomGenerator);

            var result = expression.ToSingleRegexString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsJaFran_WhenConcatExpressionContainsTwoElementReturningFranziskaAndJ()
        {
            var secondExpression = Substitute.For<IExpression>();
            secondExpression.ToRegexString().Returns("Fran");
            _expression.ToRegexString().Returns("Ja");
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression).Append(secondExpression);

            var result = concatExpression.ToSingleRegexString();

            Assert.AreEqual("JaFran", result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsUlrike_WhenConcatExpressionContainsOneElementReturningFranziska()
        {
            _expression.ToRegexString().Returns("Ulrike");
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression);

            var result = concatExpression.ToSingleRegexString();

            Assert.AreEqual("Ulrike", result);
        }
    }
}