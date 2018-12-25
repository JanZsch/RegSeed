using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;
using Regseed.Parser;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class BaseExpressionTest : BaseExpression
    {
        private IRandomGenerator _randomGenerator;
        private int _maxInverseLength;

        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _maxInverseLength = 5;
        }

        public BaseExpressionTest() : base(null)
        { }

        public BaseExpressionTest(IRandomGenerator random) : base(random)
        { }

        public override void SetExpansionLength(int expansionLength = 0)
        { }

        public override IList<IStringBuilder> Expand() =>
            new List<IStringBuilder>();

        public override IExpression GetInverse() =>
            Substitute.For<IExpression>();

        public override IExpression Clone() =>
            Substitute.For<IExpression>();

        protected override IntegerInterval GetMaxExpansionInterval() =>
            new IntegerInterval(1);

        protected override IStringBuilder ToSingleStringBuilder()
        {
            var alphabet = Substitute.For<IParserAlphabet>();
            alphabet.IsValid(Arg.Any<string>()).Returns(true);
            var charClass = new CharacterClassExpression(alphabet, _randomGenerator, _maxInverseLength);
            charClass.AddCharacters(new List<string> {"F"});
            
            return new StringBuilder(new List<CharacterClassExpression>{charClass});
        }

        [TestCase(null, 0)]
        [TestCase(-1, 0)]
        [TestCase(2, 2)]
        public void ToRegexString_CallsRandomGeneratorWithExpectedLowerBound_WhenIntegerIntervalsLowerBoundHasSpecifiedValue(int? value, int expectedValue)
        {
            var interval = new IntegerInterval();
            interval.TrySetValue(value, 12);
            
            var expression = new BaseExpressionTest(_randomGenerator)
            {
                RepeatRange = interval
            };

            expression.ToStringBuilder();

            _randomGenerator.Received(1).GetNextInteger(expectedValue, 12);
        }

        [TestCase(null, int.MaxValue)]
        [TestCase(-1, 0)]
        [TestCase(2, 2)]
        public void ToRegexString_CallsRandomGeneratorWithExpectedUpperBound_WhenIntegerIntervalsUpperBoundHasSpecifiedValue(int? value, int expectedValue)
        {
            var interval = new IntegerInterval();
            interval.TrySetValue(-2, value);

            var expression = new BaseExpressionTest(_randomGenerator)
            {
                RepeatRange = interval
            };

            expression.ToStringBuilder();

            _randomGenerator.Received(1).GetNextInteger(0, expectedValue);
        }

        [Test]
        public void ToRegexString_ReturnsSingleString3Times_WhenRepeatIntervalIs3To3()
        {
            var expression = new BaseExpressionTest(_randomGenerator) {RepeatRange = new IntegerInterval(3)};
            _randomGenerator.GetNextInteger(3, 3).Returns(3);

            var result = expression.ToStringBuilder().GenerateString();

            Assert.AreEqual("FFF", result);
            _randomGenerator.Received(1).GetNextInteger(3, 3);
        }

        [Test]
        public void ToRegexString_ReturnsSingleStringOnce_WhenNoRepeatIntervalWasProvided()
        {
            _randomGenerator.GetNextInteger(1, 1).Returns(1);
            var expression = new BaseExpressionTest(_randomGenerator);

            var result = expression.ToStringBuilder().GenerateString();

            Assert.AreEqual("F", result);
        }
    }
}