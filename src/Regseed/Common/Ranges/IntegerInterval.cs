using System;
using Regseed.Common.Resources;

namespace Regseed.Common.Ranges
{
    public class IntegerInterval : BaseRange<int?>
    {
        public IntegerInterval(int? start, int? end) : base(start, end)
        {
            if (start == null || end == null || start.Value <= end.Value) 
                return;
            
            var exceptionMessage = string.Format(ParserMessages.InvalidInterval, start.Value, end.Value);
            throw new ArgumentException(exceptionMessage);
        }
    }
}