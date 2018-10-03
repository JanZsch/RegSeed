using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.RegexSeeder.Expressions;

namespace Regseed.Test.RegexSeeder.Expressions
{
    [TestFixture]
    internal class UnionExpressionTest : UnionExpression
    {
        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _expression = Substitute.For<IExpression>();
            _expression.ToRegexString().Returns("Till");
        }

        private IRandomGenerator _randomGenerator;
        private IExpression _expression;

        public UnionExpressionTest() : base(null, null)
        {
        }

        public UnionExpressionTest(List<IExpression> expressions, IRandomGenerator random) : base(expressions, random)
        {
        }

        [Test]
        public void GetComplement_CallsGetComplementOnEachUnionElement()
        {
            var expression1 = Substitute.For<IExpression>();
            var expression2 = Substitute.For<IExpression>();
            var unionExpressions = new List<IExpression> {expression1, expression2};
            var union = new UnionExpressionTest(unionExpressions, _randomGenerator);

            union.GetComplement();

            expression1.Received(1).GetComplement();
            expression2.Received(1).GetComplement();
        }

        [Test]
        public void GetComplement_ReturnValueHasSameRepeatIntegerIntervalAsOriginal()
        {
            var repeatRange = new IntegerInterval(1, 2);
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator) {RepeatRange = repeatRange};

            var result = union.GetComplement().RepeatRange;

            Assert.AreEqual(repeatRange, result);
        }

        [Test]
        public void GetComplement_ReturnValueHasTypeUnionExpression()
        {
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator);

            var result = union.GetComplement();

            Assert.IsInstanceOf<UnionExpression>(result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsEmptyString_WhenUnionIsEmpty()
        {
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator);

            var result = union.ToSingleRegexString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsRandomUnionElement_WhenUnionContainsAtLeastTwoElements()
        {
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(2);
            var unionExpressions = new List<IExpression>
            {
                Substitute.For<IExpression>(),
                Substitute.For<IExpression>(),
                _expression,
                Substitute.For<IExpression>()
            };
            var union = new UnionExpressionTest(unionExpressions, _randomGenerator);

            var result = union.ToSingleRegexString();

            Assert.AreEqual("Till", result);
            _expression.Received(1).ToRegexString();
            _randomGenerator.Received(1).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void ToSingleRegexString_ReturnsStringValueOfFirstUnionElement_WhenUnionContainsOneElement()
        {
            var union = new UnionExpressionTest(new List<IExpression> {_expression}, _randomGenerator);

            var result = union.ToSingleRegexString();

            Assert.AreEqual("Till", result);
            _expression.Received(1).ToRegexString();
        }
    }
}