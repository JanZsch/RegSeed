using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        private int _maxExpansionLength;
        
        public IntegerInterval RepeatRange { get; set; }

        public int ExpansionLength =>
            0;

        public int MaxExpansionLength
        {
            get => _maxExpansionLength; 
            set => _maxExpansionLength = 0;
        }

        public void SetOptimalExpansionLength(int? expansionLength = null) =>
            _maxExpansionLength = 0;

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