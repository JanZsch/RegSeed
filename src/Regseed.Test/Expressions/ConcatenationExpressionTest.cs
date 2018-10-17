using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;
using Regseed.Parser;
using Regseed.Test.Mocks;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class ConcatenationExpressionTest : ConcatenationExpression
    {
        private IRandomGenerator _randomGenerator;
        private IExpression _expression;
       
        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _expression = Substitute.For<IExpression>();
        }

        public ConcatenationExpressionTest() : base(null)
        {
        }

        public ConcatenationExpressionTest(IRandomGenerator random) : base(random)
        {
        }

        [Test]
        public void GetComplement_ResultHasSameRepeatIntervalAsOriginal()
        {
            var repeatInterval = new IntegerInterval(null);
            var concatExpression = new ConcatenationExpression(_randomGenerator) {RepeatRange = repeatInterval};

            var result = concatExpression.GetComplement();

            Assert.AreEqual(repeatInterval, result.RepeatRange);
        }

        [Test]
        public void GetComplement_ReturnValueContainsTwoExpressions_WhenExpressionContainsTwoElements()
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

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsJaFran_WhenConcatExpressionContainsTwoElementReturningFranziskaAndJ()
        {
            var mock1 = new StringBuilderMock(ToCharacterClassList("Fran"));
            var mock2 = new StringBuilderMock(ToCharacterClassList("Ja"));
            var secondExpression = Substitute.For<IExpression>();
            secondExpression.ToStringBuilder().Returns(mock1);
            _expression.ToStringBuilder().Returns(mock2);
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression).Append(secondExpression);

            var result = concatExpression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("JaFran", result);
        }

        [Test]
        public void ToSingleRegexString_ReturnsUlrike_WhenConcatExpressionContainsOneElementReturningFranziska()
        {
            var mock = new StringBuilderMock(ToCharacterClassList("Ulrike"));
            _expression.ToStringBuilder().Returns(mock);
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression);

            var result = concatExpression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("Ulrike", result);
        }

        private List<CharacterClassExpression> ToCharacterClassList(string returnValue)
        {
            return returnValue.Select(x =>
            {
                var alphabet = Substitute.For<IParserAlphabet>();
                alphabet.IsValid(Arg.Any<string>()).Returns(true);

                var val = new CharacterClassExpression(alphabet, _randomGenerator);
                val.TryAddCharacters(new List<string> {x.ToString()});
                return val;
            }).ToList();
        }
    }
}