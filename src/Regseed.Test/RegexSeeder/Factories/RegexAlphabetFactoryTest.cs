using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Regseed.RegexSeeder.Factories;

namespace Regseed.Test.RegexSeeder.Factories
{
    [TestFixture]
    public class RegexAlphabetFactoryTest
    {
        [Test]
        public void Default_DoesNotThrow_WhenCalled()
        {
            Assert.DoesNotThrow(() => RegexAlphabetFactory.Default());
        }
        
        [Test]
        public void Default_ReturnedAlphabetContainsAllCapitalLetters()
        {
            var alphabet = RegexAlphabetFactory.Default();
            
            for (var i = 'A'; i <= 'Z'; i++)
                Assert.IsTrue(alphabet.IsValid(i.ToString()), $"Failed character: {i}");
        }
        
        [Test]
        public void Default_ReturnedAlphabetContainsAllLetters()
        {
            var alphabet = RegexAlphabetFactory.Default();
            
            for (var i = 'a'; i <= 'z'; i++)
                Assert.IsTrue(alphabet.IsValid(i.ToString()), $"Failed character: {i}");
        }
        
        [Test]
        public void Default_ReturnedAlphabetContainsAllDigits()
        {
            var alphabet = RegexAlphabetFactory.Default();
            
            for (var i = '0'; i <= '9'; i++)
                Assert.IsTrue(alphabet.IsValid(i.ToString()), $"Failed character: {i}");
        }
        
        [TestCase('-')]
        [TestCase('+')]
        [TestCase('#')]
        [TestCase('*')]
        [TestCase('?')]
        [TestCase(':')]
        [TestCase('.')]
        [TestCase(';')]
        [TestCase(',')]
        [TestCase('_')]
        [TestCase('!')]
        [TestCase('§')]
        [TestCase('%')]
        [TestCase('&')]
        [TestCase('/')]
        [TestCase('(')]
        [TestCase(')')]
        [TestCase('=')]
        [TestCase('\\')]
        public void Default_ReturnedAlphabetContainsSpecialCharacter(char specialCharacter)
        {
            var alphabet = RegexAlphabetFactory.Default();
            
            Assert.IsTrue(alphabet.IsValid(specialCharacter.ToString()));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Default_RegexControlCharactersHaveSpecifiedIsValidValue_WhenCalledWithArgument(bool isValid)
        {
            var defaultAlphabet = RegexAlphabetFactory.Default(isValid);
            
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("*"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("+"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("{"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("?"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("["));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("]"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("("));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid(")"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("|"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("."));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("~"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("\\"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DefaultExtendedBy_RegexControlCharactersHaveSpecifiedIsValidValue_WhenCalledWithArgument(bool isValid)
        {
            var defaultAlphabet = RegexAlphabetFactory.DefaultExtendedBy(new List<string>{"Ü"}, isValid);
            
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("*"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("+"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("{"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("?"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("["));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("]"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("("));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid(")"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("|"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("."));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("~"));
            Assert.AreEqual(isValid, defaultAlphabet.IsValid("\\"));
        }

        
        [TestCase(true)]
        [TestCase(false)]
        public void Default_NonRegexControlCharactersAreAlwaysValid_WhenCalledWithArgument(bool isValid)
        {
            var defaultAlphabet = RegexAlphabetFactory.Default(isValid);

            var result = defaultAlphabet.IsValid("J");
            
            Assert.IsTrue(result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DefaultExtendedBy_NonRegexControlCharactersAreAlwaysValid_WhenCalledWithArgument(bool isValid)
        {
            var defaultAlphabet = RegexAlphabetFactory.DefaultExtendedBy( new List<string>{"Ü"}, isValid);

            var resultJ = defaultAlphabet.IsValid("J");
            var resultUe = defaultAlphabet.IsValid("Ü");
            
            Assert.IsTrue(resultJ);
            Assert.IsTrue(resultUe);
        }

        
        [Test]
        public void Minimal_ReturnedAlphabetContainsOnlyRegexControlLetters()
        {
            var minimal = RegexAlphabetFactory.Minimal();

            var result = minimal.GetAllCharacters();
            
            Assert.True(result.Any(x => x.Equals("*")));
            Assert.True(result.Any(x => x.Equals("+")));
            Assert.True(result.Any(x => x.Equals("{")));
            Assert.True(result.Any(x => x.Equals("?")));
            Assert.True(result.Any(x => x.Equals("[")));
            Assert.True(result.Any(x => x.Equals("]")));
            Assert.True(result.Any(x => x.Equals("(")));
            Assert.True(result.Any(x => x.Equals(")")));
            Assert.True(result.Any(x => x.Equals("|")));
            Assert.True(result.Any(x => x.Equals(".")));
            Assert.True(result.Any(x => x.Equals("~")));
            Assert.True(result.Any(x => x.Equals("\\")));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void Minimal_SetsIsValidValueForRegexControlLettersToExpectedResult_WhenCalledWithExplicitIsValidValue(bool isValid)
        {
            var minimal = RegexAlphabetFactory.Minimal(isValid);

            var result = minimal.GetAllCharacters();

            foreach (var letter in result)
                Assert.AreEqual(isValid, minimal.IsValid(letter));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void MinimalExtendedBy_SetsIsValidValueForAdditionalLettersToTrue_IndependentOfIsValidValueOfRegexControlLetters(bool isValid)
        {
            var minimalExtendedBy = RegexAlphabetFactory.MinimalExtendedBy(new List<string>{"F"}, isValid);

            var result = minimalExtendedBy.IsValid("F");
            var regexControlResult = minimalExtendedBy.IsValid("{");
            
            Assert.IsTrue(result);
            Assert.AreEqual(isValid, regexControlResult);
        }
    }
}