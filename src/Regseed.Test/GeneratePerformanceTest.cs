using System;
using NUnit.Framework;

namespace Regseed.Test
{
    [TestFixture]
    public class GeneratePerformanceTest
    {
        [Test]
        public void Generate_ReturnsValueAfterAtMost600MilliSeconds_WhenResultMustContainSingleCharacterOrSpecialCharacterOrDigitAndIs14LettersLong()
        {
            const string pattern = "(.*((\\d.*[A-Z]|[A-Z].*\\d)|(\\d.*[?+!]|[!+?].*\\d)|([!+?].*[A-Z]|[A-Z].*[!+?])).*)&\\w{14}";
            
            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            var result = StopExecutionTime(() => regseed.Generate(), out _);

            Assert.LessOrEqual(result, 600.0);
        }

        [Test]
        public void Generate_ReturnsValueAfterAtMost50MilliSeconds_WhenSingleExpressionIsExcluded()
        {
            const string pattern = "vorgangsnummer\\-[0-4]\\d{1}\\-dummy&~(vorgangsnummer\\-10\\-dummy)";

            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            var result = StopExecutionTime(() => regseed.Generate(), out var generateResult);

            var numberAsString = generateResult
                .TrimStart("vorgangsnummer-".ToCharArray())
                .TrimEnd("-dummy".ToCharArray());
            var number = int.Parse(numberAsString);

            Assert.AreNotEqual(10, number);
            Assert.LessOrEqual(result, 50.0);
        }        

        [Ignore("just for now")]
        [Test]
        public void Generate_ReturnsValueAfterAtMost100MilliSeconds_WhenAllNumbersMultipleOf5UpTo50AreExcluded_Version1()
        {
            var pattern = "vorgangsnummer\\-[0-4]\\d{1}\\-dummy";

            for (var i = 0; i <= 50; i += 5)
                pattern = $"{pattern}&~(vorgangsnummer\\-{i}\\-dummy)";
            
            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            var result = StopExecutionTime(() => regseed.Generate(), out var generateResult);

            Assert.LessOrEqual(result, 100.0);
            
            var numberAsString = generateResult
                .TrimStart("vorgangsnummer-".ToCharArray())
                .TrimEnd("-dummy".ToCharArray());
            var number = int.Parse(numberAsString);

            Assert.AreNotEqual(0, number % 5);
        }        

        [Test]
        public void Generate_ReturnsValueAfterAtMost60MilliSeconds_WhenAllNumbersMultipleOf5UpTo5000AreExcluded_Version2()
        {
            const string pattern = "vorgangsnummer\\-[0-4]\\d{3}\\-dummy&~(vorgangsnummer\\-[0-4]\\d{2}[05]\\-dummy)";
           
            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            var result = StopExecutionTime(() => regseed.Generate(), out var generateResult);
           
            Assert.LessOrEqual(result, 60.0);
            
            var numberAsString = generateResult
                .TrimStart("vorgangsnummer-".ToCharArray())
                .TrimEnd("-dummy".ToCharArray());
            var number = int.Parse(numberAsString);

            Assert.AreNotEqual(0, number % 5);
        }

        [Test]
        public void Generate_ReturnsValueAfterAtMost150MilliSeconds_WhenAllNumbersMultipleOf5UpTo50AreExcluded_Version3()
        {
            var pattern = "vorgangsnummer\\-[0-4]\\d{1}\\-dummy";

            var excludedNumbers = "0";
            for (var i = 5; i <= 50; i += 5)
                excludedNumbers = $"{excludedNumbers}|{i}";

            pattern = $"{pattern}&~(vorgangsnummer\\-({excludedNumbers})\\-dummy)";
            
            var regseed = new RegSeed().EnableStandardWildCards();
            var loadResult = regseed.TryLoadRegexPattern(pattern);

            Assert.IsTrue(loadResult.IsSuccess);

            var result = StopExecutionTime(() => regseed.Generate(), out var generateResult);

            Assert.LessOrEqual(result, 150.0);
            
            var numberAsString = generateResult
                .TrimStart("vorgangsnummer-".ToCharArray())
                .TrimEnd("-dummy".ToCharArray());
            var number = int.Parse(numberAsString);

            Assert.AreNotEqual(0, number % 5, $"Generated result: {generateResult}");
        }        
        
        private static double StopExecutionTime(Func<string> action, out string result)
        {
            var start = DateTime.Now;
            
            result = action.Invoke();

            var end = DateTime.Now;

            return (end - start).TotalMilliseconds;
        }
    }
}