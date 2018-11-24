using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal interface IExpression
    {
        IntegerInterval RepeatRange { get; set; }

        IList<IStringBuilder> Expand();
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
        IExpression Clone();
    }
}