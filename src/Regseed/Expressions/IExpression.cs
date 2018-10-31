using System.Collections.Generic;
using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    public interface IExpression
    {
        IntegerInterval RepeatRange { get; set; }

        IList<IStringBuilder> Expand();
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
    }
}