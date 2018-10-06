using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    public interface IExpression
    {
        IntegerInterval RepeatRange { get; set; }
        
        string ToRegexString();
        IExpression GetComplement();
    }
}