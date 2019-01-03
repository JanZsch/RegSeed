using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Ranges;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        public IntegerInterval RepeatRange { get; set; }

        public int? ExpansionLength =>
            0;

        public IntegerInterval MaxExpansionRange
        {
            get => new IntegerInterval(0);
            set { }
        }

        public void SetExpansionLength(int expansionLength)
        {}

        public IList<IStringBuilder> Expand() =>
            new List<IStringBuilder>{StringBuilder.Empty};

        public IStringBuilder ToStringBuilder() => 
            StringBuilder.Empty;

        public IExpression GetInverse()
        {
            var expressionFactory = RegularExpressionFactory.GetFactoryAsSingleton();
            
            var repeatRange = new IntegerInterval();
            repeatRange.TrySetValue(1, expressionFactory.MaxInverseLength);

            var characterClassExpression = expressionFactory.GetFullCharacterClassExpression();
            characterClassExpression.RepeatRange = repeatRange;
            
            return characterClassExpression;
        }

        public IExpression Clone() =>
            this;
    }
}