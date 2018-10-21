using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    public interface IExpression
    {
        IntegerInterval RepeatRange { get; set; }
        
        IStringBuilder ToStringBuilder();
        IExpression GetInverse();
    }
}