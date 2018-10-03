using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.RegexSeeder.Expressions;

namespace Regseed.Test.RegexSeeder.Expressions
{
    [TestFixture]
    internal class BaseExpressionTest : BaseExpression
    {
        private IRandomGenerator _randomGenerator;
        
        public BaseExpressionTest() : base(null)
        {}
        
        public BaseExpressionTest(IRandomGenerator random) : base(random)
        {}

        public override IExpression GetComplement() => Substitute.For<IExpression>();
        protected override string ToSingleRegexString() => "F";

        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
        }

        [Test]
        public void ToRegexString_ReturnsSingleStringOnce_WhenNoRepeatIntervalWasProvided()
        {
            _randomGenerator.GetNextInteger(1, 2).Returns(1);
            var expression = new BaseExpressionTest(_randomGenerator);

            var result = expression.ToRegexString();
            
            Assert.AreEqual("F", result);
        }
        
        [Test]
        public void ToRegexString_ReturnsSingleString3Times_WhenRepeatIntervalIs3To3()
        {
            var expression = new BaseExpressionTest(_randomGenerator);
            expression.RepeatRange = new IntegerInterval(3,3);
            _randomGenerator.GetNextInteger(3, 4).Returns(3);

            var result = expression.ToRegexString();
            
            Assert.AreEqual("FFF", result);
            _randomGenerator.Received(1).GetNextInteger(3, 4);
        }
        
        [TestCase(null, 0)]
        [TestCase(-1, 0)]
        [TestCase(2, 2)]
        public void ToRegexString_CallsRandomGeneratorWithExpectedLowerBound_WhenIntegerIntervalsLowerBoundHasSpecifiedValue(int? value, int expectedValue)
        {
            var expression = new BaseExpressionTest(_randomGenerator)
            {
                RepeatRange = new IntegerInterval(value, 12)
            };

            expression.ToRegexString();
            
            _randomGenerator.Received(1).GetNextInteger(expectedValue, 13);
        }        
        [TestCase(null, int.MaxValue)]
        [TestCase(-1, 0)]
        [TestCase(2, 3)]
        public void ToRegexString_CallsRandomGeneratorWithExpectedUpperBound_WhenIntegerIntervalsUpperBoundHasSpecifiedValue(int? value, int expectedValue)
        {
            var expression = new BaseExpressionTest(_randomGenerator)
            {
                RepeatRange = new IntegerInterval(-2, value)
            };

            expression.ToRegexString();
            
            _randomGenerator.Received(1).GetNextInteger(0, expectedValue);
        }
    }
}