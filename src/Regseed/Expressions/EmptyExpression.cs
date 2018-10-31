using System.Collections.Generic;
using Regseed.Common.Ranges;
using Regseed.Factories;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }

        public IList<IStringBuilder> Expand()
        {
            return new List<IStringBuilder>{StringBuilder.Empty};
        }

        public IStringBuilder ToStringBuilder()
        {
            return StringBuilder.Empty;
        }

        public IExpression GetInverse()
        {
            return this;
        }
    }
}