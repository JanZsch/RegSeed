using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal interface IExpression
    {
        int ExpansionLength { get; }
        int MaxExpansionLength { get; set; }
        IntegerInterval RepeatRange { get; set; }

        void SetOptimalExpansionLength(int? expansionLength = null);
        IList<IStringBuilder> Expand();
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
        IExpression Clone();
    }
}