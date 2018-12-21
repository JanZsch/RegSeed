using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class UnionExpressionTest : UnionExpression
    {
        private IStringBuilder _stringBuilder;
        
        [SetUp]
        public void SetUp()
        {
            _stringBuilder = Substitute.For<IStringBuilder>();
            _stringBuilder.GenerateString().Returns("Claudia");
            
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _expression = Substitute.For<IExpression>();
            _expression.ToStringBuilder().Returns(_stringBuilder);
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
        public void GetInverse_CallsGetComplementOnEachUnionElement()
        {
            var expression1 = Substitute.For<IExpression>();
            var expression2 = Substitute.For<IExpression>();
            var unionExpressions = new List<IExpression> {expression1, expression2};
            var union = new UnionExpressionTest(unionExpressions, _randomGenerator);

            union.GetInverse();

            expression1.Received(1).GetInverse();
            expression2.Received(1).GetInverse();
        }

        [Test]
        public void GetInverse_ReturnValueHasSameRepeatIntegerIntervalAsOriginal()
        {
            var repeatRange = new IntegerInterval();
            repeatRange.TrySetValue(1, 2);
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator) {RepeatRange = repeatRange};

            var result = union.GetInverse().RepeatRange;

            Assert.AreEqual(repeatRange, result);
        }

        [Test]
        public void GetInverse_ReturnValueHasTypeUnionExpression()
        {
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator);

            var result = union.GetInverse();

            Assert.IsInstanceOf<IntersectionExpression>(result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsEmptyString_WhenUnionIsEmpty()
        {
            var union = new UnionExpressionTest(new List<IExpression>(), _randomGenerator);

            var result = union.ToSingleStringBuilder().GenerateString();

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

            var result = union.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("Claudia", result);
            _expression.Received(1).ToStringBuilder();
            _randomGenerator.Received(1).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void ToSingleRegexString_ReturnsStringValueOfFirstUnionElement_WhenUnionContainsOneElement()
        {
            var union = new UnionExpressionTest(new List<IExpression> {_expression}, _randomGenerator);

            var result = union.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("Claudia", result);
            _expression.Received(1).ToStringBuilder();
        }

        [Test]
        public void Clone_ReturnsNewConcatenationInstanceWithSameValues()
        {
            var expression = Substitute.For<IExpression>();
            var union = new UnionExpression(new List<IExpression>{expression},  _random)
            {
                RepeatRange = new IntegerInterval()
            };
            union.RepeatRange.TrySetValue(1, 3);

            var result = union.Clone();
            
            Assert.AreNotEqual(union, result);
            Assert.AreEqual(union.RepeatRange.Start, result.RepeatRange.Start);
            Assert.AreEqual(union.RepeatRange.End, result.RepeatRange.End);
            expression.Received(1).Clone();
        }
        
        [Test]
        public void Expand_ReturnsListContainingFourStringBuilder_WhenUnionContainsTwoInitialIntersectionExpressionsReturningTwoStringBuildersEachOnExpand()
        {
            var intersect1 = Substitute.For<IExpression>();
            intersect1.Expand()
                      .Returns(new List<IStringBuilder> {Substitute.For<IStringBuilder>(), Substitute.For<IStringBuilder>()});
            var intersect2 = Substitute.For<IExpression>();
            intersect2.Expand()
                      .Returns(new List<IStringBuilder> {Substitute.For<IStringBuilder>(), Substitute.For<IStringBuilder>()});
            var expression = new UnionExpression(new List<IExpression>{intersect1, intersect2}, _random);

            var result = expression.Expand();
            
            Assert.AreEqual(4, result.Count);
        }
    }
}