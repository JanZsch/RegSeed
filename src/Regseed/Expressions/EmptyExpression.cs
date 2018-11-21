using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }

        public IList<IStringBuilder> Expand() =>
            new List<IStringBuilder>{StringBuilder.Empty};

        public IStringBuilder ToStringBuilder() => 
            StringBuilder.Empty;

        public IExpression GetInverse() =>
            this;
    }
}