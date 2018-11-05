using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;
using Regseed.Factories;

namespace Regseed.Test
{
    [TestFixture]
    public class RegSeedIntegrationTest : RegSeed
    {
        [SetUp]
        public void SetUp()
        {
            _random = Substitute.For<IRandomGenerator>();
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(x => x[0]);
        }

        [TestCase("", "")]
        [TestCase("()", "")]
        [TestCase("a", "a")]
        [TestCase("ab", "ab")]
        [TestCase("(a)", "a")]
        [TestCase("([a-a])", "a")]
        [TestCase("([a-a][b-b])", "ab")]
        [TestCase("[a-a][b-b]", "ab")]
        [TestCase("([a][b])", "ab")]
        [TestCase("[a][b]", "ab")]
        [TestCase("[a]", "a")]
        [TestCase("[aa]", "a")]
        [TestCase("(j[a-a]n)", "jan")]
        [TestCase("(a|a|a)", "a")]
        [TestCase("(a)|[a-a]|a", "a")]
        [TestCase("a{3}b", "aaab")]
        [TestCase("^ulrike", "ulrike")]
        [TestCase("jan$", "jan")]
        public void Generate_ReturnsDeterministicString(string regex, string expectedValue)
        {
            var loadResult = TryLoadRegexPattern(regex);

            var result = Generate();

            Assert.IsTrue(loadResult.IsSuccess);
            Assert.AreEqual(expectedValue, result);
        }

        [TestCase("a|b")]
        [TestCase("(a|b)")]
        [TestCase("(a|b){5}")]
        public void Generate_ReturnsAllPossibleResults_WhenCalledSufficientlyOften(string pattern)
        {
            _random = new RandomGenerator(new Random());
            TryLoadRegexPattern(pattern);

            var result = false;
            var regexResult = string.Empty;
            for (var i = 0; i < 10; i++)
            {
                regexResult = Generate();
                result = result || regexResult.Contains("b");
            }

            Assert.IsTrue(result, $"{pattern}: last regexResult: {regexResult}");
        }

        [TestCase("[^{0}]", "F")]
        [TestCase("~~F", "F")]
        [TestCase("~[{0}]", "F")]
        [TestCase("~[{0}]ra", "Fra")]
        [TestCase("~[{0}]{{2}}", "FF")]
        public void Generate_ReturnsExpectedResult_WhenRegexIsComplementOfCharacterClassWithInverseLength1AndContainingCharactersButA(string regexPattern, string expectedResult)
        {
            _random = new RandomGenerator(new Random());
            var alphabet = RegexAlphabetFactory.Default();
            var alphabetCharacters = alphabet.GetAllCharacters();
            var alphabetAsString = alphabetCharacters.Aggregate(string.Empty, (current, character) => $"{current}\\{character}");
            var alphabetWithoutF = alphabetAsString.Replace("\\F", string.Empty);
            var alphabetWithoutR = alphabetAsString.Replace("\\R", string.Empty);
            var regex = string.Format(regexPattern, alphabetWithoutF, alphabetWithoutR);
            var regseed = new RegSeed(_random, alphabet, 1);

            var loadResult = regseed.TryLoadRegexPattern(regex);
            var result = regseed.Generate();

            Assert.IsTrue(loadResult.IsSuccess);
            Assert.AreEqual(expectedResult, result, regex);
        }

        [TestCase("{0}|{1}")]
        [TestCase("({0}|{1})")]
        public void Generate_ReturnsLetterFOrR_WhenRegexIsFOrR(string regexPattern)
        {
            var countOfF = 0; 
            var countOfR = 0; 
            _random = new RandomGenerator(new Random());
            var regex = string.Format(regexPattern, "F", "R");

            var loadResult = TryLoadRegexPattern(regex);

            Assert.IsTrue(loadResult.IsSuccess, regexPattern);
            
            for (var i = 0; i < 50; i++)
            {
                var result = Generate();

                Assert.IsTrue(result.Contains("F") || result.Contains("R"), result);

                if (result.Equals("F"))
                    countOfF++;
                else if (result.Equals("R"))
                    countOfR++;
            }
            
            Assert.Greater(countOfF, 0, countOfF.ToString());
            Assert.Greater(countOfR, 0, countOfR.ToString());
            Assert.AreEqual(50, countOfF+countOfR);
        }
        
        [TestCase("[a-z]{0,5}", false)]
        [TestCase("ul rike", false)]
        [TestCase("ul\nrike", false)]
        [TestCase("ul\trike", false)]
        [TestCase("(A|[a-m])+[a-z]{2,4}", true)]
        [TestCase("[^m-z0-9A-Z]+", true)]
        [TestCase("[aaabbbaaa]+", true)]
        [TestCase("[f-i]+", true)]
        [TestCase("([0-9]|[a-z]|Ulrike){1,2}", false)]
        [TestCase("(F|R)", false)]
        [TestCase("(Fr([aA]n|A[nN])ziska){1,4}", false)]
        [TestCase(@"^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$", false)]
        public void Generate_ReturnsStringMatchingProvidedPattern(string pattern, bool restrictUppBound)
        {
            var realRandomGenerator = new RandomGenerator(new Random());
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(x =>
            {
                var upperBound =  restrictUppBound && (int) x[1] > 5 ? 5 : (int)x[1];
                return realRandomGenerator.GetNextInteger((int) x[0], upperBound);

            });
            var regex = new Regex($"^{pattern}$");
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            for (var i = 0; i < 25; i++)
            {
                var result = Generate();

                Assert.IsTrue(regex.IsMatch(result), $"pattern: {pattern}  result: {result}");
            }
        }

        [TestCase("{", 0)]
        [TestCase("a{a}", 1)]
        [TestCase("a-z", 0)]
        [TestCase("[(x)]", 1)]
        [TestCase("(0|1)]", 5)]
        [TestCase("[0|1]", 2)]
        [TestCase("a++", 2)]
        [TestCase("a+*", 2)]
        [TestCase("a{3,1}", 1)]
        [TestCase("[x-a]", 2)]
        [TestCase("a{,1", 1)]
        [TestCase("a{1x,1}", 3)]
        public void TryLoadRegexPattern_ReturnsFailureResultWithExpectedErrorPosition_WhenRegexPatternFaulty(string faultyPattern, int expectedErrorPosition)
        {
            var result = TryLoadRegexPattern(faultyPattern);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedErrorPosition, result.Position);
        }

        [Test]
        public void Generate_ReturnedStringContainsOnlyCharactersFromHToL_WhenRegexIsRepeatedRangeForCharactersHToL()
        {
            var validCharacters = new List<char> {'H', 'I', 'J', 'K', 'L'};
            const string pattern = "[H-L]{5}";
            var reseed = new RegSeed(pattern);

            var result = reseed.Generate();

            foreach (var character in result)
                Assert.IsTrue(validCharacters.Contains(character), $"'{character}' is no valid character");
        }

        [Test]
        public void Generate_ReturnsDifferentResult_WhenCalledTwice()
        {
            _random = new RandomGenerator(new Random());
            TryLoadRegexPattern(".{10}");

            var firstCallResult = Generate();
            var secondCallResult = Generate();

            Assert.AreNotEqual(firstCallResult, secondCallResult, $"Output: {firstCallResult} {secondCallResult}");
        }

        [Test]
        public void Generate_ReturnsFOrEmptyString_WhenRegexIsFQuestionMark()
        {
            _random = new RandomGenerator(new Random());
            const string regex = "F?";

            var loadResult = TryLoadRegexPattern(regex);
            var result = Generate();

            Assert.IsTrue(loadResult.IsSuccess);
            Assert.IsTrue(result.Contains("F") || string.IsNullOrEmpty(result));
        }

        [Test]
        public void Generate_ReturnsResultWithDifferentLength_WhenCalledTwiceAndRegexAllowsArbitraryResultLength()
        {
            _random = new RandomGenerator(new Random());
            const string regex = "a{0,100}";
            TryLoadRegexPattern(regex);

            var result1 = Generate().Length;
            var result2 = Generate().Length;

            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void Interval_BindsStrongerThanUnion_WhenRegexContainsIntervalWithUnion()
        {
            _random = new RandomGenerator(new Random());
            const string regex = "Ja|Fran{2}";
            var regseed = new RegSeed(regex);

            for (var i = 0; i < 25; i++)
            {
                var result = regseed.Generate();
                
                Assert.AreNotEqual("JaJa", result);
            }
        }

        [TestCase("jan&[Jj][Aa][Nn]","jan")]
        [TestCase("jan&Fra","")]
        [TestCase("[Ulr][Ulr][Ulr]&Ul","")]
        [TestCase("ulri\\-[0-9]\\-ke&ulri\\-9\\-ke","ulri-9-ke")]
        [TestCase("x|jan&~jan","x")]
        [TestCase("franziska&franziska","franziska")]
        [TestCase("Fr([Aa][Nn]&an)ziska","Franziska")]
        [TestCase("Fr((AN|an)&an)ziska","Franziska")]
        [TestCase("[Jj]irko[Jj]irko&Jirko{1,3}","")]
        [TestCase("[Jj]irko[Jj]irko&(Jirko){1,3}","JirkoJirko")]
        [TestCase("aa&a{1,3}","aa")]
        [TestCase("jan&ja&j","")]
        [TestCase("[Jja]an&j(ja|[aA]|jan)n","jan")]
        [TestCase("a|[a-c]&a","a")]
        [TestCase("a(~(b|c))c&a([a-c])c", "aac")]
        [TestCase("[fr][fr]&[fr]r&f[fr]","fr")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsIntersection(string pattern, string expectedResult)
        {
            _random = new RandomGenerator(new Random());
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess, pattern);
            var result = Generate();

            Assert.AreEqual(expectedResult, result, "Result was: {0} .", result);
        }

        [TestCase("~(f[0-8])&(f[0-9])","f9")]
        [TestCase("~(c[0-9])&~(f[0-8])&[cf][0-9]","f9")]
        [TestCase("~(f{1,2})&f{1,3}","fff")]
        [TestCase("~f&f{1,2}","ff")]
        [TestCase("F(R|r)a&~(FRa)","Fra")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsIntersectionInCombinationWithInverse(string pattern, string expectedResult)
        {
            _random = new RandomGenerator(new Random());
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess, "Faulty pattern: {0}", pattern);
            var result = Generate();

            Assert.AreEqual(expectedResult, result, "Result was: {0} .", result);
        }
        
        [Test]
        public void Generate_DoesNotThrow_WhenRegexContainsJustComplement()
        {
            _random = new RandomGenerator(new Random());
            TryLoadRegexPattern("~(trump)");

            Assert.DoesNotThrow(() => Generate());
        }

        
        [TestCase("a[a-zA-Z]c&a[Bb]c","abc", "aBc")]
        [TestCase("[Jja]an&(Jan|jan)","jan", "Jan")]
        [TestCase("a|b|AB&ab","a", "b")]
        public void Generate_ReturnsEquallyDistributedResults_WhenCalledFiveHundredTimes(string pattern, string resultA, string resultB)
        {
            const int totalRuns = 500;
            _random = new RandomGenerator(new Random());
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess, pattern);
            var countA = 0;
            var countB = 0;

            for (var i = 0; i < totalRuns; i++)
            {
                var result = Generate();

                if (result.Equals(resultA))
                    countA++;
                else if (result.Equals(resultB))
                    countB++;
            }

            var relativeFrequencyA = countA / (double) totalRuns;
            var relativeFrequencyB = countB / (double) totalRuns;

            Assert.AreEqual(totalRuns, countA + countB);
            Assert.IsTrue(Math.Abs(relativeFrequencyA - 0.5) <= 0.05, $"frequency {resultA}: {relativeFrequencyA}   frequency {resultB}: {relativeFrequencyB}");
        }
        
        [TestCase("~b&b", "")]
        [TestCase("A|~b&b", "A")]
        [TestCase("A|a(~[bc])c&a(c|b)c", "A")]
        public void Generate_AlwaysReturnsExpectedResult_WhenExpressionIsUnionAndOneExpressionReturnsEmptyString(string pattern, string expectedResult)
        {
            const int totalRuns = 50;
            _random = new RandomGenerator(new Random());
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            for (var i = 0; i < totalRuns; i++)
            {
                var result = Generate();

                Assert.AreEqual(expectedResult, result);
            }
        }
    }
}