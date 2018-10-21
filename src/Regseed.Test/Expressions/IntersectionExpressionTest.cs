using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Expressions;
using Regseed.Parser;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class IntersectionExpressionTest : IntersectionExpression
    {
        private IRandomGenerator _randomGenerator;
        private IParserAlphabet _alphabet;
        
        [SetUp]
        public void SetUp()
        {
            _randomGenerator = Substitute.For<IRandomGenerator>();
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(x => (int)x[1]-(int)x[0] == 1 ? x[0] : 0);
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _alphabet.GetAllCharacters().Returns(new List<string> {"F", "r", "a"});
        }

        public IntersectionExpressionTest() : base(null, null)
        {
        }
        
        public IntersectionExpressionTest(IList<IExpression> concatExpressions, IRandomGenerator random) : base(concatExpressions, random)
        {
        }

        [Test]
        public void ToSingleStringBuilder_ReturnedStringBuilderGeneratesEmptyString_WhenNoConcatExpressionsIsEmpty()
        {
            var expression = new IntersectionExpressionTest(new List<IExpression>(), _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleStringBuilder_ReturnedStringBuilderGeneratesEmptyString_WhenNoConcatExpressionsIsNull()
        {
            var expression = new IntersectionExpressionTest(null, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();
            
            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleStringBuilder_ReturnedStringBuilderReturnsF_WhenSingleCharacterClassWithFContainedInIntersectionExpression()
        {
            var expression = new IntersectionExpressionTest(new List<IExpression>{ToCharacterClass("F")}, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.IsNotNull(result);
            Assert.AreEqual("F", result);
        }
        
        [Test]
        public void ToSingleStringBuilder_ReturnedStringBuilderReturnsIntersection_WhenTwoCharacterClassesContainedInIntersectionExpression()
        {
            var firstCharClass = ToCharacterClass("F", "f");
            var secondCharClass = ToCharacterClass("f");
            var expression = new IntersectionExpressionTest(new List<IExpression>{firstCharClass, secondCharClass}, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.IsNotNull(result);
            Assert.AreEqual("f", result);
        }
        
        [Test]
        public void ToSingleStringBuilder_ReturnedStringBuilderReturnsIntersection_WhenTwoConcatExpressionsContainedInIntersectionExpression()
        {
            var concat1 = new ConcatenationExpression(_randomGenerator);
            concat1.Append(ToCharacterClass("F", "f")).Append(ToCharacterClass("R", "r"));
            var concat2 = new ConcatenationExpression(_randomGenerator);
            concat2.Append(ToCharacterClass("F")).Append(ToCharacterClass("r"));
            var expression = new IntersectionExpressionTest(new List<IExpression>{concat1, concat2}, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.IsNotNull(result);
            Assert.AreEqual("Fr", result);
        }
        
        [Test]
        public void ToSingleStringBuilder_ReturnsEmpty_WhenTwoConcatExpressionsContainedInIntersectionExpressionWithLength2And1Respectively()
        {
            var concat1 = new ConcatenationExpression(_randomGenerator);
            concat1.Append(ToCharacterClass("F", "f")).Append(ToCharacterClass("R", "r"));
            var concat2 = new ConcatenationExpression(_randomGenerator);
            concat2.Append(ToCharacterClass("F", "a"));
            var expression = new IntersectionExpressionTest(new List<IExpression>{concat1, concat2}, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.IsNotNull(result);
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetComplement_ReturnsStringBuilderReturningComplementOfSingleCharacterClass_WhenIntersectionExpressionContainsSingleElementWithCharactersRA()
        {
            var concat1 = new ConcatenationExpression(_randomGenerator);
            concat1.Append(ToCharacterClass("r", "a"));
            var expression = new IntersectionExpressionTest(new List<IExpression>{concat1}, _randomGenerator);

            var result = expression.GetComplement().ToStringBuilder().GenerateString();

            Assert.IsNotNull(result);
            Assert.AreEqual("F", result);            
        }
      
        [Test]
        public void GetComplement_ReturnsOriginalIntersectResult_WhenCalledTwice()
        {
            var concat1 = new ConcatenationExpression(_randomGenerator);
            concat1.Append(ToCharacterClass("F", "a", "r"));
            var concat2 = new ConcatenationExpression(_randomGenerator);
            concat2.Append(ToCharacterClass("F"));
            var expression = new IntersectionExpressionTest(new List<IExpression>{concat1, concat2}, _randomGenerator);

            var result = expression.GetComplement().GetComplement().ToStringBuilder().GenerateString();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("F", result);            
        }
        
        private CharacterClassExpression ToCharacterClass(params string[] characters)
        {
            var charClass = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClass.TryAddCharacters(characters.ToList());
            return charClass;
        }
    }
}