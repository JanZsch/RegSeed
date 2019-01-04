using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;
using Regseed.Common.Random;
using Regseed.Common.Ranges;
using Regseed.Parser;

namespace Regseed.Expressions
{
    internal class CharacterClassExpression : BaseExpression
    {
        protected readonly IParserAlphabet _alphabet;
        private List<string> _characterList = new List<string>();
        private readonly IDictionary<string, string> _literals = new Dictionary<string, string>();

        protected CharacterClassExpression() : base(null)
        {
        }

        public CharacterClassExpression(IParserAlphabet alphabet, IRandomGenerator random) : base(random)
        {
            _alphabet = alphabet ?? throw new ArgumentNullException();
        }

        public int GetCharacterCount() =>
            _characterList.Count;

        public override void SetExpansionLength(int expansionLength = 0) =>
            ExpansionLength = expansionLength;

        public override IList<IStringBuilder> Expand()
        {
            var returnList = new List<IStringBuilder>();
            
            var singleStringBuilder = ToSingleStringBuilder();

            RepeatRange.ToExpansionBounds(out var lowerBound, out var upperBound);

            for (var stringBuilderLength = lowerBound; stringBuilderLength <= upperBound; stringBuilderLength++)
            {
                IStringBuilder finalStringBuilder = StringBuilder.Empty;
                finalStringBuilder = finalStringBuilder.ConcatWith(singleStringBuilder, stringBuilderLength);
                
                returnList.Add(finalStringBuilder);
            }

            return returnList;
        }

        public override IExpression GetInverse(int inverseLength)
        {
            return inverseLength <= 1 
                ? (IExpression) GetComplement() 
                : WrapAllNonComplementInversesUpToMaxInverseLengthAround(inverseLength, GetComplement());
        }

        public override IExpression Clone()
        {
            var clone = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange?.Clone(),
                ExpansionLength = ExpansionLength
            };
            
            clone.AddCharacters(_characterList);
            
            return clone;
        }

        public virtual CharacterClassExpression GetComplement()
        {
            var complementCharacters = _alphabet.GetAllCharacters().Where(x => !_literals.ContainsKey(x)).ToList();
            var complement = new CharacterClassExpression(_alphabet, _random);
            
            complement.AddCharacters(complementCharacters);

            return complement;
        }
        
        public string GetRandomCharacter()
        {
            if (_characterList.Count == 0)
                return string.Empty;
            
            return  _characterList.Count == 1 
            ? _characterList[0]
            :_characterList[_random.GetNextInteger(0, _characterList.Count - 1)];
        }

        public virtual CharacterClassExpression GetIntersection(CharacterClassExpression charClass)
        {
            if(charClass == null || !charClass._characterList.Any() || !_characterList.Any())
                return new CharacterClassExpression(_alphabet, _random);

            ClassifyListsByLength(this, charClass, out var shortDict, out var longDict);

            var intersectList = shortDict.Where(x => longDict.ContainsKey(x.Key))
                                         .Select(y => y.Key)
                                         .ToList();
            
            var intersection = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            intersection.AddCharacters(intersectList);
            
            return intersection;    
        }

        public virtual CharacterClassExpression GetUnion(CharacterClassExpression charClass)
        {
            if (charClass?.GetType() == typeof(CompleteCharacterClassExpression))
                return charClass;            
            
            var union = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            
            union.AddCharacters(_characterList);
    
            if(charClass != null)
                union.AddCharacters(charClass._characterList);
            
            return union;
        }
        
        public virtual void AddCharacters(IEnumerable<string> characters)
        {
            foreach (var letter in characters)
                _literals[letter] = null;

            _characterList = _literals.Keys.ToList();
        }

        protected override IntegerInterval GetMaxExpansionInterval() =>
            RepeatRange;

        protected override IStringBuilder ToSingleStringBuilder() =>
            new StringBuilder(new List<CharacterClassExpression> {this});
        
        protected UnionExpression WrapAllNonComplementInversesUpToMaxInverseLengthAround(int inverseLength, IExpression complement = null)
        {
            var minimalLengthTwoRange = new IntegerInterval();
            minimalLengthTwoRange.TrySetValue(2, inverseLength);

            var atLeastLengthTwoWords = new CompleteCharacterClassExpression(_alphabet, _random) {RepeatRange = minimalLengthTwoRange};

            var inverseExpressions = new List<IExpression>
            {
                new EmptyExpression(_alphabet, _random),
                atLeastLengthTwoWords
            };

            if(complement != null)
                inverseExpressions.Add(complement);
            
            return new UnionExpression(inverseExpressions, _random);            
        }

        private static void ClassifyListsByLength(CharacterClassExpression charClass1, CharacterClassExpression charClass2, out IDictionary<string, string> shortDict, out IDictionary<string, string> longDict)
        {
            if (charClass1._literals.Count < charClass2._literals.Count)
            {
                shortDict = charClass1._literals;
                longDict = charClass2._literals;
            }
            else
            {
                shortDict = charClass2._literals;
                longDict = charClass1._literals;
            }
        }
    }
}