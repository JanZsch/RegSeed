using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;
using Regseed.Factories;
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
        public void GetInverse_ReturnsIntersectExpressionWithTwoSubexpressionsEachHavingRepeatIntervalNull_WhenOriginalExpressionContainsSingleSubExpressionWithRepeatIntervalFrom1To2()
        {
            var repeatInterval = new IntegerInterval();
            repeatInterval.TrySetValue(1, 2);
            var inverseSubExpression = Substitute.For<IExpression>();
            var subExpression = Substitute.For<IExpression>();
            subExpression.GetInverse().Returns(inverseSubExpression);
            subExpression.RepeatRange.Returns(repeatInterval);
            var concatExpression = new ConcatenationExpression(_randomGenerator);
            concatExpression.Append(subExpression);            

            var result = concatExpression.GetInverse();
            Assert.IsInstanceOf<IntersectionExpression>(result);

            var resultAsList = ((IntersectionExpression) result).ToConcatExpressionList();
            Assert.AreEqual(2, resultAsList.Count);
            Assert.IsNull(resultAsList[0].RepeatRange);
            Assert.IsNull(resultAsList[1].RepeatRange);
        }

        [Test]
        public void GetInverse_ReturnsIntersectExpressionWithTwoSubexpressionsOfLength3_WhenOriginalExpressionContains3SubExpressionsWithMiddleExpressionOfRepeatIntervalFrom1To2()
        {
            var repeatInterval = new IntegerInterval();
            repeatInterval.TrySetValue(1, 2);
            var inverseSubExpression = Substitute.For<IExpression>();
            inverseSubExpression.GetInverse().Returns(Substitute.For<IExpression>());
            var subExpression = Substitute.For<IExpression>();
            subExpression.GetInverse().Returns(inverseSubExpression);
            subExpression.RepeatRange.Returns(repeatInterval);
            var concatExpression = new ConcatenationExpression(_randomGenerator);
            concatExpression.Append(inverseSubExpression).Append(subExpression).Append(inverseSubExpression);            

            var result = concatExpression.GetInverse();
            Assert.IsInstanceOf<IntersectionExpression>(result);

            var resultAsList = ((IntersectionExpression) result).ToConcatExpressionList();
            Assert.AreEqual(2, resultAsList.Count);
            Assert.AreEqual(3, ((UnionExpression) resultAsList[0]).ToIntersectionExpressionList().Count);
            Assert.AreEqual(4, ((UnionExpression) resultAsList[1]).ToIntersectionExpressionList().Count);
        }
        
        [Test]
        public void GetInverse_ReturnValueContainsTwoExpressions_WhenExpressionContainsTwoElements()
        {
            var secondExpression = Substitute.For<IExpression>();
            var concatExpression = new ConcatenationExpressionTest(_randomGenerator);
            concatExpression.Append(_expression).Append(secondExpression);

            concatExpression.GetInverse();

            secondExpression.Received(1).GetInverse();
            _expression.Received(1).GetInverse();
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

        [Test]
        public void Expand_ReturnsListContaining3Elements_WhenSingleNotExpandableExpressionWithRepeatRangeFrom1To3IsContained()
        {
            var expression = Substitute.For<IExpression>();
            expression.RepeatRange = new IntegerInterval();
            expression.RepeatRange.TrySetValue(1, 3);
            expression.Expand().Returns(new List<IStringBuilder> {new StringBuilderMock(new List<CharacterClassExpression>())});
            var concat = new ConcatenationExpression(_random);
            concat.Append(expression);
            
            var result = concat.Expand();
            
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void Expand_ReturnsListContaining3ElementsOfLength4_WhenOneExpandableExpressionWith3PossibleSubexpressionsIsContained()
        {
            var mockA = new StringBuilderMock(ToCharacterClassList("a"));
            var mock1 = new StringBuilderMock(ToCharacterClassList("1"));
            var mock2 = new StringBuilderMock(ToCharacterClassList("2"));
            var mock3 = new StringBuilderMock(ToCharacterClassList("3"));
            
            var simpleExpression = Substitute.For<IExpression>();
            simpleExpression.RepeatRange = new IntegerInterval(1);
            
            simpleExpression.Expand().Returns(new List<IStringBuilder> { mockA });
            
            var expandableExpression = Substitute.For<IExpression>();
            expandableExpression.RepeatRange = new IntegerInterval(1);
            expandableExpression.Expand().Returns(new List<IStringBuilder> { mock1, mock2, mock3 });

            var concat = new ConcatenationExpression(_random);
            concat.Append(simpleExpression).Append(expandableExpression).Append(simpleExpression).Append(simpleExpression);
            
            var result = concat.Expand();
            
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a1aa", result[0].GenerateString());
            Assert.AreEqual("a2aa", result[1].GenerateString());
            Assert.AreEqual("a3aa", result[2].GenerateString());
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