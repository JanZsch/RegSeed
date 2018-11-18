namespace Regseed.Common.Ranges
{
    internal static class IntegerIntervalExtension
    {
        public static void ToBounds(this IntegerInterval interval, out int lowerBound, out int upperBound)
        {
            var repeatRange = interval ?? new IntegerInterval(1);
            lowerBound = repeatRange.Start == null || repeatRange.Start < 0 ? 0 : repeatRange.Start.Value;
            upperBound = repeatRange.End ?? int.MaxValue - 1;
            upperBound = upperBound + 1 < 0 ? 0 : upperBound + 1;
        }
    }
}