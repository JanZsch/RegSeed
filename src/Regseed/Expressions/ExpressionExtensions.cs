using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal static class ExpressionExtensions
    {
        public static void ToRepeatExpansionBounds(this IExpression expression, out int lowerBound, out int upperBound)
        {
            expression.RepeatRange.ToExpansionBounds(out lowerBound, out upperBound);
            
            if (expression.ExpansionLength != null &&  expression.ExpansionLength < upperBound)
                upperBound = expression.ExpansionLength.Value;
        }
    }
}