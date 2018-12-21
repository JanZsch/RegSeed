using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        private IntegerInterval _maxExpansionInterval;
        
        public IntegerInterval RepeatRange { get; set; }

        public int ExpansionLength =>
            0;

        public IntegerInterval MaxExpansionRange
        {
            get => _maxExpansionInterval; 
            set => _maxExpansionInterval = new IntegerInterval(0);
        }

        public void SetExpansionLength(int expansionLength) =>
            _maxExpansionInterval = new IntegerInterval(0);

        public IList<IStringBuilder> Expand() =>
            new List<IStringBuilder>{StringBuilder.Empty};

        public IStringBuilder ToStringBuilder() => 
            StringBuilder.Empty;

        public IExpression GetInverse() =>
            this;

        public IExpression Clone() =>
            this;
    }
}