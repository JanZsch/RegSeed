namespace Regseed.Common.Ranges
{
    internal static class IntegerIntervalExtension
    {
        public static void ToExpansionBounds(this IntegerInterval interval, out int lowerBound, out int upperBound)
        {
            var repeatRange = interval ?? new IntegerInterval(1);
            lowerBound = repeatRange.Start == null || repeatRange.Start < 0 ? 0 : repeatRange.Start.Value;
            upperBound = repeatRange.End ?? int.MaxValue;
            upperBound = upperBound < 0 ? 0 : upperBound;
        }

        public static void ToLowerExpansionBound(this IntegerInterval interval, out int lowerBound)
        {
            var repeatRange = interval ?? new IntegerInterval(1);
            lowerBound = repeatRange.Start == null || repeatRange.Start < 0 ? 0 : repeatRange.Start.Value;
        }
    }
}