using Regseed.Common.Ranges;

namespace Regseed.RegexSeeder.Expressions
{
    internal class EmptyExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }

        public string ToRegexString()
        {
            return string.Empty;
        }

        public IExpression GetComplement()
        {
            return this;
        }
    }
}