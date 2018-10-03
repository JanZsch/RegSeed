using System;
using Regseed.RegexSeeder;

namespace Regseed
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            var regseed = new RegSeed("[a-zA-Z0-9_\\+&$§#]{10,15}");

            var repeat = true;
            var result = string.Empty;
            while (repeat)
            {
                var execTime = GetExecutionTime(() => { result = regseed.Generate(); });

                Console.WriteLine($"result: {result} execution time: {execTime}");
                var key = Console.ReadKey();

                repeat = key.Key != ConsoleKey.Q;
            }
        }

        private static double GetExecutionTime(Action action)
        {
            var start = DateTime.Now;
            action.Invoke();
            var stop = DateTime.Now;

            return (stop - start).TotalMilliseconds;
        }
    }
}