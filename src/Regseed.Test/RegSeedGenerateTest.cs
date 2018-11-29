using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Random;

namespace Regseed.Test
{
    [TestFixture]
    public class RegSeedGenerateTest : RegSeed
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
        [TestCase("^ulrike", "ulrike")]
        [TestCase("jan$", "jan")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsCharacterClasses(string regex, string expectedValue)
        {
            var loadResult = TryLoadRegexPattern(regex);

            var result = Generate();

            Assert.IsTrue(loadResult.IsSuccess);
            Assert.AreEqual(expectedValue, result);
        }

        [TestCase("^a{3}b$", "aaab")]
        [TestCase("a{2}", "aa")]
        [TestCase("a{2}b{2}", "aabb")]
        [TestCase("Ja{2,2}n", "Jaan")]
        [TestCase("Ja{0}n", "Jn")]
        [TestCase("Ja{1}n", "Jan")]
        [TestCase("J(aa){2}n", "Jaaaan")]
        [TestCase("J(a|a){2}n", "Jaan")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsConcatenation(string regex, string expectedValue)
        {
            var loadResult = TryLoadRegexPattern(regex);

            var result = Generate();

            Assert.IsTrue(loadResult.IsSuccess);
            Assert.AreEqual(expectedValue, result);
        }        
        
        [TestCase("jan&[Jj][Aa][Nn]","jan")]
        [TestCase("jan&Fra","")]
        [TestCase("[Ulr][Ulr][Ulr]&Ul","")]
        [TestCase("ulri\\-[0-9]\\-ke&ulri\\-9\\-ke","ulri-9-ke")]
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

        [TestCase("aabb&a{2}b{2}","aabb")]
        [TestCase("a{2}b{2}&aabb","aabb")]
        [TestCase(".{2}a.{3}&12a345","12a345")]
        [TestCase(".{0,2}a.{0,3}&12a34","12a34")]
        [TestCase(".*&a","a")]
        [TestCase("(.*)&a","a")]
        [TestCase(".{0,2}a.{0,3}&12a34","12a34")]
        [TestCase("1(a|ab|[1-2]){0,2}a.{0,3}&12a34","12a34")]
        [TestCase(".{0,1}a.{0,2}&1a3","1a3")]
        [TestCase(".*r.*&Ul[Rr]ike","Ulrike")]
        [TestCase("[a-Z+*/.]{0,10}r.{0,10}&Marlene","Marlene")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsIntersectionAndConcatenation(string pattern, string expectedResult)
        {
            var regseed = new RegSeed();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess, pattern);
            var result = regseed.Generate();

            Assert.AreEqual(expectedResult, result, "Result was: {0} .", result);
        }
        
        [TestCase(0,7)]
        public void Generate_ReturnsStringMatchingPattern_WhenResultMustContainSingleCharacterOrSpecialCharacterOrDigitAndIsBetweenMinAndMaxCharactersLong(int min, int max)
        {
            var pattern = $"(.*((\\d.*[A-Z]|[A-Z].*\\d)|(\\d.*[?+!]|[!+?].*\\d)|([!+?].*[A-Z]|[A-Z].*[!+?])).*)&\\w{{{min},{max}}}";
            
            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);
            var result = regseed.Generate();

            var specialCharMatcher = new Regex(".*[!+?].*");
            var digitMatcherCharMatcher = new Regex(".*\\d.*");
            var capitalCharMatcher = new Regex(".*[A-Z].*");
            var matchResult = (specialCharMatcher.IsMatch(result) || 
                               digitMatcherCharMatcher.IsMatch(result) ||
                               capitalCharMatcher.IsMatch(result)) && result.Length <= max && result.Length >= min;
            
            Assert.IsTrue(matchResult, "Result was: {0} .", result);
        }
        
        [TestCase("~(f[0-8])&(f[0-9])","f9")]
        [TestCase("~(c[0-9])&~(f[0-8])&[cf][0-9]","f9")]
        [TestCase("~(f{1,2})&f{1,3}","fff")]
        [TestCase("~f&f{1,2}","ff")]
        [TestCase("F(R|r)a&~(FRa)","Fra")]
        [TestCase("x|jan&~jan","x")]
        public void Generate_ReturnsExpectedResult_WhenRegexContainsIntersectionInCombinationWithInverse(string pattern, string expectedResult)
        {
            _random = new RandomGenerator(new Random());
            var loadResult = TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess, "Faulty pattern: {0}", pattern);
            var result = Generate();

            Assert.AreEqual(expectedResult, result, "Result was: {0} .", result);
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
            var regseed = new RegSeed(_random, alphabet).SetMaxCharClassInverseLength(1);

            var loadResult = regseed.TryLoadRegexPattern(regex);
            Assert.IsTrue(loadResult.IsSuccess, $"faulty pattern: {regex}");
            
            var result = regseed.Generate();

            Assert.AreEqual(expectedResult, result, regex);
        }

        [TestCase("a{1}", 1, 1)]
        [TestCase("a{2}", 2, 2)]
        [TestCase("a{,4}", 0, 4)]
        [TestCase(".{0,4}", 0, 4)]
        [TestCase("a{4,6}", 4, 6)]
        [TestCase("a?", 0, 1)]
        [TestCase("u|ll|iii", 1, 3)]
        public void Generate_ReturnsAllPossibleLengths_WhenRegexContainsVariableLengthExpressionAndIsCalledSufficientlyOften(string regex, int lowerBound, int upperBound)
        {
            const int runs = 100;
            var counter = new int[upperBound-lowerBound+1];
            var regseed = new RegSeed();
            
            var loadResult = regseed.TryLoadRegexPattern(regex);
            Assert.IsTrue(loadResult.IsSuccess);
            
            for (var i = 0; i < runs; i++)
            {
                var result = regseed.Generate();
                counter[result.Length-lowerBound]++;
            }

            var totalResults = 0;

            foreach (var count in counter)
            {
                totalResults += count;
                Assert.Greater(count, 0);
            }
            
            Assert.AreEqual(runs, totalResults);
        }
        
        [TestCase("{0}|{1}", 0.5, 0.5, 0)]
        [TestCase("({0}|{1})", 0.5, 0.5, 0)]
        [TestCase("{0}|{1}|{2}", 0.3333, 0.3333, 0.3333)]
        [TestCase("{0}|{0}|{2}", 0.6666, 0, 0.3333)]
        [TestCase("{0}&{1}|{1}|{2}", 0, 0.5, 0.5)]
        [TestCase("[{0}-{0}]&{1}|{1}|{2}", 0, 0.5, 0.5)]
        [TestCase("[{2}-{1}]&{0}|{0}|{2}", 0.6666, 0, 0.3333)]
        public void Generate_ResultsAreEssentiallyEquallyDistributed_WhenRegexContainsUnion(string regexPattern, double frequencyF, double frequencyR, double frequencyA)
        {
            const int runs = 800;
            var countOfF = 0; 
            var countOfR = 0; 
            var countOfA = 0; 
            _random = new RandomGenerator(new Random());
            var regex = string.Format(regexPattern, "F", "R", "A");

            var loadResult = TryLoadRegexPattern(regex);

            Assert.IsTrue(loadResult.IsSuccess, regexPattern);
            
            for (var i = 0; i < runs; i++)
            {
                var result = Generate();

                Assert.IsTrue(result.Contains("F") || result.Contains("R") || result.Contains("A"), result);

                if (result.Equals("F"))
                    countOfF++;
                else if (result.Equals("R"))
                    countOfR++;
                else if (result.Equals("A"))
                    countOfA++;
            }

            var derivationFromExpectedResultF = Math.Abs(countOfF / (double) runs - frequencyF);
            var derivationFromExpectedResultR = Math.Abs(countOfR / (double) runs - frequencyR);
            var derivationFromExpectedResultA = Math.Abs(countOfA / (double) runs - frequencyA);
            
            Assert.IsTrue(derivationFromExpectedResultF < 0.05, $"Actual derivation F: {derivationFromExpectedResultF}");
            Assert.IsTrue(derivationFromExpectedResultR < 0.05, $"Actual derivation R: {derivationFromExpectedResultR}");
            Assert.IsTrue(derivationFromExpectedResultA < 0.05, $"Actual derivation A: {derivationFromExpectedResultA}");
            Assert.AreEqual(runs, countOfF+countOfR+countOfA);
        }
        
        [Test]
        public void Generate_ReturnsAnythingButLettersABC_WhenPatternIsCharacterClassInverseOfABC()
        {
            const string pattern = "[^ABC]";
            var regseed = new RegSeed();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 200; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsFalse(result[0] == 'A' || result[0] == 'B' || result[0] == 'C', $"Faulty result: {new string(result)}");
            }
        }
        
        [Test]
        public void Generate_ReturnsAnythingButDigit_WhenPatternIsCharacterClassInverseOfDigitRange0to9()
        {
            const string pattern = "[^0-9]";
            var regseed = new RegSeed();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 200; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsFalse(char.IsDigit(result[0]), $"Faulty result: {new string(result)}");
            }
        }
        
        [TestCase("a[a-zA-Z]c&a[Bb]c","abc", "aBc")]
        [TestCase("[Jja]an&(Jan|jan)","jan", "Jan")]
        [TestCase("a|b|AB&ab","a", "b")]
        [TestCase("[abaaaa]","a", "b")]
        public void Generate_ReturnsEquallyDistributedResults_WhenRegexContainsCharacterClassesAndOrIntersection(string pattern, string resultA, string resultB)
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
       
        [TestCase("[a-z]{0,5}", false)]
        [TestCase("ul rike", false)]
        [TestCase("ul\nrike", false)]
        [TestCase("ul\trike", false)]
        [TestCase("(A|[a-m])+[a-z]{2,4}", true)]
        [TestCase("[^m-z0-9A-Z]+", true)]
        [TestCase("[m-z0-9A-Z^]+", true)]
        [TestCase("[aaabbbaaa]+", true)]
        [TestCase("[f-i]+", true)]
        [TestCase("([0-9]|[a-z]|Ulrike){1,2}", false)]
        [TestCase("(F|R)", false)]
        [TestCase("(Fr([aA]n|A[nN])ziska){1,4}", false)]
        [TestCase(@"^([0-9A-Fa-f]{8}[\-][0-9A-Fa-f]{4}[\-][0-9A-Fa-f]{4}[\-][0-9A-Fa-f]{4}[\-][0-9A-Fa-f]{12})$", false)]
        [TestCase(@"^([0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12})$", false)]
        public void Generate_ReturnsStringMatchingProvidedPattern(string pattern, bool restrictUpperBound)
        {
            var realRandomGenerator = new RandomGenerator(new Random());
            _random.GetNextInteger(Arg.Any<int>(), Arg.Any<int>()).Returns(x =>
            {
                var upperBound =  restrictUpperBound && (int) x[1] > 5 ? 5 : (int)x[1];
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
        public void Interval_BindsStrongerThanUnion_WhenRegexContainsIntervalWithUnion()
        {
            _random = new RandomGenerator(new Random());
            const string regex = "Ja|Uli{2}";
            var regseed = new RegSeed(regex);

            for (var i = 0; i < 25; i++)
            {
                var result = regseed.Generate();
                
                Assert.AreNotEqual("JaJa", result);
            }
        }
        
        [Test]
        public void Generate_DoesNotThrow_WhenRegexContainsJustComplement()
        {
            _random = new RandomGenerator(new Random());
            TryLoadRegexPattern("~(trump)");

            Assert.DoesNotThrow(() => Generate());
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

        [Test]
        public void Generate_ReturnsDigit_WhenEnableWildCardsIsTrueAndPatternIsDigitWildCard()
        {
            const string pattern = "\\d";
            var regseed = new RegSeed().EnableStandardWildCards();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 20; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length);
                Assert.IsTrue(char.IsDigit(result[0]));
            }
        }
        
        [Test]
        public void Generate_ReturnsCharacter_WhenEnableWildCardsIsTrueAndPatternIsCharacterWildCard()
        {
            const string pattern = "\\w";
            var regseed = new RegSeed().EnableStandardWildCards();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 50; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsTrue(char.IsLetter(result[0]) || char.IsDigit(result[0]) || result[0] == '_', $"Faulty result: {new string(result)}");
            }
        }
        
        [Test]
        public void Generate_ReturnsAnythingButCharacter_WhenEnableWildCardsIsTrueAndPatternIsNotCharacterWildCardAndMaxInverseLengthIsOne()
        {
            const string pattern = "\\W";
            var regseed = new RegSeed().SetMaxCharClassInverseLength(1).EnableStandardWildCards();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 50; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsFalse(char.IsLetter(result[0]) || char.IsDigit(result[0]) || result[0] == '_', $"Faulty result: {new string(result)}");
            }
        }
        
        [Test]
        public void Generate_ReturnsAnythingButDigit_WhenEnableWildCardsIsTrueAndPatternIsNotDigitWildCardAndMaxInverseLengthIsOne()
        {
            const string pattern = "\\D";
            var regseed = new RegSeed().EnableStandardWildCards().SetMaxCharClassInverseLength(1);
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 50; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsFalse(char.IsDigit(result[0]));
            }
        }
        
        [Test]
        public void Generate_ReturnsAnythingButWhiteSpace_WhenEnableWildCardsIsTrueAndPatternIsNotWhiteSpaceWildCardAndMaxInverseLengthIsOne()
        {
            const string pattern = "\\S";
            var regseed = new RegSeed().EnableStandardWildCards().SetMaxCharClassInverseLength(1);
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 50; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsFalse(char.IsWhiteSpace(result[0]));
            }
        }
        
        [Test]
        public void Generate_ReturnsWhiteSpace_WhenEnableWildCardsIsTrueAndPatternIsWhiteSpaceWildCard()
        {
            const string pattern = "\\s";
            var regseed = new RegSeed().EnableStandardWildCards();
            regseed.TryLoadRegexPattern(pattern);

            for (var i = 0; i < 50; i++)
            {
                var result = regseed.Generate().ToCharArray();
                
                Assert.AreEqual(1, result.Length, $"Faulty result: {new string(result)}");
                Assert.IsTrue(char.IsWhiteSpace(result[0]), $"Faulty result: {new string(result)}");
            }
        }

        [TestCase("[?]", "?")]
        [TestCase("[+]", "+")]
        [TestCase("[[]", "[")]
        [TestCase("[$]", "$")]
        [TestCase("[{]", "{")]
        [TestCase("[}]", "}")]
        [TestCase("[,]", ",")]
        [TestCase("[.]", ".")]
        [TestCase("[*]", "*")]
        public void Generate_ReturnsExpectedResult_WhenPatternsContainsUnescapedSpecialCharacterInCharacterClass(string pattern, string expectedResult)
        {
            var regseed = new RegSeed();
            
            var loadResult = regseed.TryLoadRegexPattern(pattern);
            Assert.IsTrue(loadResult.IsSuccess);
            
            var result = regseed.Generate();
            
            Assert.AreEqual(expectedResult, result);
        }
        
        [TestCase("[\\^]", "^")]
        [TestCase("[\\]]", "]")]
        [TestCase("[\\-]", "-")]
        public void Generate_ReturnsExpectedResult_WhenPatternsContainsEscapedSpecialCharacterInCharacterClass(string pattern, string expectedResult)
        {
            var regseed = new RegSeed();
            
            var loadResult = regseed.TryLoadRegexPattern(pattern);
            Assert.IsTrue(loadResult.IsSuccess);
            
            var result = regseed.Generate();
            
            Assert.AreEqual(expectedResult, result);
        }
    }
}