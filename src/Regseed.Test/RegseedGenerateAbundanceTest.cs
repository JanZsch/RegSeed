using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Regseed.Common.Random;

namespace Regseed.Test
{
    [TestFixture]
    public class RegseedGenerateAbundanceTest : RegSeed
    {
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

        [Test]
        public void Generate_ReturnsAllPossibleValues_WhenPatternContainsConcatenationOfInverse()
        {
            const int runs = 100;
            const string pattern = "(~f){1,2}&[Ff][rf]{0,1}";
            var resultCount = new Dictionary<string, int>();
            var regseed = new RegSeed(pattern);

            for (var i = 0; i < runs; i++)
            {
                var result = regseed.Generate();

                if (resultCount.ContainsKey(result))
                    resultCount[result]++;
                else
                    resultCount.Add(result, 1);
            }
            
            Assert.AreEqual(5, resultCount.Count);

            var results = resultCount.Values.Sum();
            
            Assert.AreEqual(100, results);
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
            const int runs = 1000;
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
        
        [TestCase("a[a-zA-Z]c&a[Bb]c","abc", "aBc")]
        [TestCase("[Jja]an&(Jan|jan)","jan", "Jan")]
        [TestCase("a|b|AB&ab","a", "b")]
        [TestCase("[abaaaa]","a", "b")]
        public void Generate_ReturnsEquallyDistributedResults_WhenRegexContainsCharacterClassesAndOrIntersection(string pattern, string resultA, string resultB)
        {
            const int totalRuns = 750;
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

            var relativeFrequencyADeviation = RelativeDeviationFromEqualDistribution(totalRuns, 2, countA);
            var relativeFrequencyBDeviation = RelativeDeviationFromEqualDistribution(totalRuns, 2, countB);
            
            Assert.AreEqual(totalRuns, countA + countB);
            Assert.IsTrue( relativeFrequencyADeviation <= 0.05, $"frequency {resultA}: {relativeFrequencyADeviation}   frequency {resultB}: {relativeFrequencyBDeviation}");
        }

        [Ignore("Takes up to 1 min to execute")]
        [Test]
        public void Generate_ReturnsAllPossibleResultsEquallyDistributed_WhenResultMustContainSingleCharacterOrSpecialCharacterOrDigitAndIs3LettersLong()
        {
            const int runs = 30000;
            const double maxRelativeDeviation = 0.075;
            const int expectedDifferentResults = 24;
            const double maxAbsoluteDeviation = maxRelativeDeviation * runs / expectedDifferentResults;
            const string pattern = "(.*(([01].*[AB]|[AB].*[01])|([01].*[?+]|[+?].*[01])|([+?].*[AB]|[AB].*[+?])).*)&[01AB+?]{2}";
            var resultDistribution = new Dictionary<string, int>();
            
            var regseed = new RegSeed();
            var loadPatterResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadPatterResult.IsSuccess);
            
            for (var run = 0; run < runs; run++)
            {
                var result = regseed.Generate();

                Assert.IsNotEmpty(result);
                
                if (resultDistribution.TryGetValue(result, out var value))
                    resultDistribution[result] = value + 1;
                else
                    resultDistribution.Add(result, 1);
            }

            Assert.AreEqual(expectedDifferentResults, resultDistribution.Count);
            
            foreach (var (key, count) in resultDistribution)
                Console.WriteLine($"key: {key}  Count: {count}");
            
            foreach (var (key, count) in resultDistribution)
            {
                var deviationFromEqualDistribution = Math.Abs(count - (double) runs / expectedDifferentResults);
                Assert.LessOrEqual(deviationFromEqualDistribution, maxAbsoluteDeviation, $"Key: {key} Count: {count}  expected Count: {runs / (double) expectedDifferentResults} +/- {maxAbsoluteDeviation}");
            }
        }
        
        private static double RelativeDeviationFromEqualDistribution(int runs, int categories, int absoluteCount)
        {
            var relativeFrequencyA = absoluteCount / (double) runs;
            return Math.Abs(relativeFrequencyA - 1.0 / categories);
        }
    }
}