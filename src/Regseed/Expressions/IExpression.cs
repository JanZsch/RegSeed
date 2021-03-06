using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal interface IExpression
    {
        int? ExpansionLength { get; }
        IntegerInterval MaxExpansionRange { get; set; }
        IntegerInterval RepeatRange { get; set; }

        void SetExpansionLength(int expansionLength = 0);
        IList<IStringBuilder> Expand();
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
        IExpression Clone();
    }
}