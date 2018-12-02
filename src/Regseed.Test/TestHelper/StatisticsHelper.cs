using System;
using System.Collections.Generic;
using System.Linq;

namespace Regseed.Test.TestHelper
{
    public static class StatisticsHelper
    {
        public static bool IsEquallyDistributed(List<List<int>> frequencies, int runs, double maxDerivation, out Tuple<int, int, double> faultyFrequency)
        {
            faultyFrequency = null;

            if (!frequencies.Any())
                return false;

            var expectedRelativeFrequency = 1.0 / frequencies[0].Count;

            for (var i = 0; i < frequencies.Count; i++)
            {
                var totalCount = 0;

                for (var j = 0; j < frequencies[i].Count; j++)
                {
                    var count = frequencies[i][j];
                    totalCount += count;
                    var relativeFrequency = count / (double) runs;
                    var derivationFromExpectedResult = Math.Abs(relativeFrequency - expectedRelativeFrequency);

                    if (derivationFromExpectedResult <= maxDerivation) 
                        continue;
                    
                    faultyFrequency = new Tuple<int, int, double>(i, j, derivationFromExpectedResult);
                    return false;
                }

                if (totalCount != runs)
                    return false;
            }

            return true;
        }
    }
}