using System;
using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Resources;

namespace Regseed.Expressions
{
    internal class InverseExpression : BaseExpression
    {
        private readonly int _inverseLengthOffSet;
        private readonly IExpression _toInvertExpression;
        
        public InverseExpression(IExpression toInvertedExpression, IRandomGenerator random, int inverseLengthOffset) : base(random)
        {
            _toInvertExpression = toInvertedExpression;
            _inverseLengthOffSet = inverseLengthOffset;
        }

        public override void SetExpansionLength(int expansionLength = 0)
        {
            ExpansionLength = expansionLength;
            _toInvertExpression.SetExpansionLength(expansionLength);
        }

        public override IList<IStringBuilder> Expand()
        {
            if(ExpansionLength == null)
                throw new ArgumentNullException(nameof(ExpansionLength), RegSeedErrorMessages.ExpandInverseOnlyWhenExpansionLengthSet);

            var inverse = _toInvertExpression.GetInverse(ExpansionLength.Value + 1);
            inverse.SetExpansionLength(ExpansionLength.Value);

            return inverse.Expand();
        }

        public override IExpression GetInverse(int inverseLength) =>
            _toInvertExpression;

        public override IExpression Clone()
        {            
            var clone = new InverseExpression(_toInvertExpression.Clone(), _random, _inverseLengthOffSet);
         
            if(ExpansionLength != null)
                clone.SetExpansionLength(ExpansionLength.Value);

            return clone;
        }

        protected override IntegerInterval GetMaxExpansionInterval()
        {
            _toInvertExpression.MaxExpansionRange.ToUpperExpansionBounds(out var upper);
            
            var expansionRange = new IntegerInterval();

            if (int.MaxValue - upper <= _inverseLengthOffSet)
                expansionRange.TrySetValue(0, int.MaxValue);
            else
                expansionRange.TrySetValue(0, upper + _inverseLengthOffSet);

            return expansionRange;
        }

        protected override IStringBuilder ToSingleStringBuilder() =>
            StringBuilder.Empty;
    }
}