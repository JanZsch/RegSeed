using System.Collections.Generic;
using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal interface IExpression
    {
        IntegerInterval RepeatRange { get; set; }

        IList<IStringBuilder> Expand();
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
    }
}