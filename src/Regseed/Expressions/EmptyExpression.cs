using System.Collections.Generic;
using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }

        public IStringBuilder ToStringBuilder()
        {
            return StringBuilder.Empty;
        }

        public IExpression GetComplement()
        {
            return this;
        }
    }
}