using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Expressions;
using Regseed.Parser;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    internal class CharacterClassExpressionTest : CharacterClassExpression
    {
        [SetUp]
        public void Setup()
        {
            _alphabet = Substitute.For<IParserAlphabet>();
            _alphabet.IsValid(Arg.Any<string>()).Returns(true);
            _randomGenerator = Substitute.For<IRandomGenerator>();

            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(x => (int) x[1] - (int) x[0] == 0 ? x[0] : 0);
        }

        private new IParserAlphabet _alphabet;
        private IRandomGenerator _randomGenerator;

        public CharacterClassExpressionTest()
        {
        }

        public CharacterClassExpressionTest(IParserAlphabet alphabet, IRandomGenerator random) : base(alphabet, random)
        {
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenAlphabetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var unused = new CharacterClassExpression(null, _randomGenerator);
            });
        }

        [Test]
        public void GetInverse_ReturnsF_WhenCharacterClassContainsAllLettersButF()
        {
            _randomGenerator = new RandomGenerator(new Random());
            _alphabet.GetAllCharacters().Returns(new List<string> {"J", "F", "r", "a", "n"});
            var expression = new CharacterClassExpression(_alphabet, _randomGenerator);
            expression.AddCharacters(new List<string> {"J", "r", "a", "n"});

            var complement = expression.GetInverse(1);
            var result = complement.ToStringBuilder().GenerateString();

            Assert.AreEqual("F", result);
        }

        [Test]
        public void Clone_ReturnsNewConcatenationInstanceWithSameValues()
        {
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var charClass = new CharacterClassExpression(_alphabet, _randomGenerator)
            {
                RepeatRange = new IntegerInterval()
            };
            charClass.RepeatRange.TrySetValue(1, 3);
            charClass.AddCharacters(new List<string> {"f"});

            var result = charClass.Clone();

            Assert.AreNotEqual(charClass, result);
            Assert.AreEqual(charClass.RepeatRange.Start, result.RepeatRange.Start);
            Assert.AreEqual(charClass.RepeatRange.End, result.RepeatRange.End);
            Assert.AreEqual("f", result.ToStringBuilder().GenerateString());
        }


        [Test]
        public void ToSingleStringBuilder_DoesNotCallRandomGenerator_WhenCharacterClassEmpty()
        {
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToSingleStringBuilder_ReturnsEmptyString_WhenCharacterClassContainsOneCharacter()
        {
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);
            expression.AddCharacters(new List<string> {"U"});

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("U", result);
            _randomGenerator.Received(0).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void ToSingleStringBuilder_ReturnsL_WhenRandomGeneratorReturns2AndSecondLetterInListIsL()
        {
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            var expression = new CharacterClassExpressionTest(_alphabet, _randomGenerator);
            expression.AddCharacters(new List<string> {"U", "L"});

            var result = expression.ToSingleStringBuilder().GenerateString();

            Assert.AreEqual("L", result);
            _randomGenerator.Received(1).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void GetIntersection_ReturnsF_WhenCharacterClassContainsFfAndIsIntersectedWithF()
        {
            var charClassFf = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassFf.AddCharacters(new List<string> {"F", "f"});
            var charClassF = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassF.AddCharacters(new List<string> {"F"});

            var result1 = charClassFf.GetIntersection(charClassF).ToStringBuilder().GenerateString();
            var result2 = charClassF.GetIntersection(charClassFf).ToStringBuilder().GenerateString();

            Assert.AreEqual("F", result1, "charClassFf.GetIntersection(charClassF)");
            Assert.AreEqual("F", result2, "charClassF.GetIntersection(charClassFf)");
        }

        [Test]
        public void GetIntersection_ReturnsEmptyString_WhenCharacterClassContainsFfAndIsIntersectedWithNull()
        {
            var charClassFf = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassFf.AddCharacters(new List<string> {"F", "f"});

            var result1 = charClassFf.GetIntersection(null).ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result1);
        }

        [Test]
        public void GetIntersection_ReturnsEmptyString_WhenCharacterClassContainsFfAndIsIntersectedWithEmptyClass()
        {
            var charClassFf = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassFf.AddCharacters(new List<string> {"F", "f"});
            var charClassEmpty = new CharacterClassExpression(_alphabet, _randomGenerator);

            var result1 = charClassFf.GetIntersection(charClassEmpty).ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result1);
        }

        [Test]
        public void GetIntersection_ReturnsEmptyString_WhenCharacterClassIsEmptyAndIsIntersectedWithNonEmptyClass()
        {
            var charClassFf = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassFf.AddCharacters(new List<string> {"F", "f"});
            var charClassEmpty = new CharacterClassExpression(_alphabet, _randomGenerator);

            var result1 = charClassEmpty.GetIntersection(charClassFf).ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result1);
        }

        [Test]
        public void GetIntersection_ReturnsEmptyString_WhenIntersectedCharacterClassesContainDifferentCharacters()
        {
            var charClassFr = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassFr.AddCharacters(new List<string> {"F", "r"});
            var charClassJa = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassJa.AddCharacters(new List<string> {"J", "a"});

            var result = charClassJa.GetIntersection(charClassFr).ToStringBuilder().GenerateString();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetCharacter_ReturnsEmptyString_WhenCharacterClassIsEmpty()
        {
            var charClassEmpty = new CharacterClassExpression(_alphabet, _randomGenerator);

            var result = charClassEmpty.GetRandomCharacter();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetCharacter_ReturnsFirstCharacter_WhenCharacterClassContainsSingleCharacter()
        {
            var charClassF = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassF.AddCharacters(new List<string> {"F"});

            var result = charClassF.GetRandomCharacter();

            Assert.AreEqual("F", result);
            _randomGenerator.Received(0).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void GetCharacter_ReturnsThirdCharacter_WhenCharacterClassContainsThreeCharactersAndRandomGeneratorReturns2()
        {
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(2);
            var charClassF = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassF.AddCharacters(new List<string> {"F", "r", "a"});

            var result = charClassF.GetRandomCharacter();

            Assert.AreEqual("a", result);
            _randomGenerator.Received(1).GetNextInteger(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void GetUnion_ReturnsCharactersJAN_WhenCharacterClassContainsJAAndIsUnitedWithCharacterClassContainingAN()
        {
            var charClassJA = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassJA.AddCharacters(new List<string> {"J", "A"});
            var charClassNA = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassNA.AddCharacters(new List<string> {"N", "A"});

            var result = charClassJA.GetUnion(charClassNA);

            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
            Assert.AreEqual("J", result.GetRandomCharacter());
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
            Assert.AreEqual("A", result.GetRandomCharacter());
            _randomGenerator.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(2);
            Assert.AreEqual("N", result.GetRandomCharacter());
        }

        [Test]
        public void GetUnion_ReturnsCharactersJ_WhenCharacterClassContainsJndIsUnitedWithNull()
        {
            var charClassJA = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassJA.AddCharacters(new List<string> {"J"});

            var result = charClassJA.GetUnion(null);

            Assert.AreEqual("J", result.GetRandomCharacter());
        }

        [Test]
        public void GetUnion_ReturnsCharactersJ_WhenCharacterClassContainsJndIsUnitedWithEmptyCharClass()
        {
            var charClassJA = new CharacterClassExpression(_alphabet, _randomGenerator);
            charClassJA.AddCharacters(new List<string> {"J"});

            var result = charClassJA.GetUnion(new CharacterClassExpression(_alphabet, _randomGenerator));

            Assert.AreEqual("J", result.GetRandomCharacter());
        }

        [TestCaseSource(nameof(GetCharacterCountTestData))]
        public void GetCharacterCount_ReturnsNumberOfPossibleGetCharacterOutComes(IList<string> characters, int expectedValue)
        {
            var charClass = new CharacterClassExpression(_alphabet, _random);
            charClass.AddCharacters(characters);

            var result = charClass.GetCharacterCount();

            Assert.AreEqual(expectedValue, result);
        }

        private static IEnumerable<object[]> GetCharacterCountTestData()
        {
            yield return new object[] {new List<string>(), 0};
            yield return new object[] {new List<string> {"a"}, 1};
            yield return new object[] {new List<string> {"a", "b"}, 2};
        }

        [Test]
        public void Expand_ReturnsSingleElementListContainingStringBuilderRepresentingCharacterClass()
        {
            var charClass = new CharacterClassExpression(_alphabet, _random);
            charClass.AddCharacters(new List<string> {"a"});

            var result = charClass.Expand();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a", result.FirstOrDefault()?.GenerateString());
        }
    }
}