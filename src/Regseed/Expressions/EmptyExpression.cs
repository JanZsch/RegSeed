using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Parser;

namespace Regseed.Expressions
{
    internal class EmptyExpression : IExpression
    {
        private readonly IParserAlphabet _alphabet;
        private readonly IRandomGenerator _random;
        public IntegerInterval RepeatRange { get; set; }

        public int? ExpansionLength =>
            0;

        public IntegerInterval MaxExpansionRange => 
            new IntegerInterval(0);

        public EmptyExpression(IParserAlphabet alphabet, IRandomGenerator random)
        {
            _alphabet = alphabet;
            _random = random;
        }

        public void SetExpansionLength(int expansionLength)
        {}

        public IList<IStringBuilder> Expand() =>
            new List<IStringBuilder>{StringBuilder.Empty};

        public IStringBuilder ToStringBuilder() => 
            StringBuilder.Empty;

        public IExpression GetInverse(int inverseLength)
        {
            var repeatRange = new IntegerInterval();
            repeatRange.TrySetValue(1, inverseLength);

            var characterClassExpression = new CompleteCharacterClassExpression(_alphabet, _random) {RepeatRange = repeatRange};

            return characterClassExpression;
        }

        public IExpression Clone() =>
            this;
    }
}