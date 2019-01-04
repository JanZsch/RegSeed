using System.Collections.Generic;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Parser;

namespace Regseed.Expressions
{
    internal class CompleteCharacterClassExpression : CharacterClassExpression
    {
        public CompleteCharacterClassExpression(IParserAlphabet alphabet, IRandomGenerator random) : base(alphabet, random)
        {
            base.AddCharacters(alphabet.GetAllCharacters());
        }

        public override IExpression GetInverse(int inverseLength) => 
            WrapAllNonComplementInversesUpToMaxInverseLengthAround(inverseLength);

        public override CharacterClassExpression GetUnion(CharacterClassExpression charClass) =>
            this;

        public override void AddCharacters(IEnumerable<string> characters)
        {
        }

        public override CharacterClassExpression GetComplement() => new CharacterClassExpression(_alphabet, _random);

        public override CharacterClassExpression GetIntersection(CharacterClassExpression charClass) =>
            (CharacterClassExpression) charClass.Clone();

        protected override IStringBuilder ToSingleStringBuilder() =>
            new StringBuilder(new List<CharacterClassExpression> {new CompleteCharacterClassExpression(_alphabet, _random)});
    }
}