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
        private readonly int _maxInverseLength;
        private readonly IParserAlphabet _alphabet;
        private List<string> _characterList = new List<string>();
        private readonly IDictionary<string, string> _literals = new Dictionary<string, string>();

        protected CharacterClassExpression() : base(null)
        {
        }

        public CharacterClassExpression(IParserAlphabet alphabet, IRandomGenerator random, int maxInverseLength) : base(random)
        {
            _alphabet = alphabet ?? throw new ArgumentNullException();
            _maxInverseLength = maxInverseLength;
        }

        public int GetCharacterCount() =>
            _characterList.Count;

        public override void SetExpansionLength(int expansionLength = 0) =>
            ExpansionLength = expansionLength;

        public override IList<IStringBuilder> Expand()
        {
            var returnList = new List<IStringBuilder>();
            
            var singleStringBuilder = ToSingleStringBuilder();

            this.ToRepeatExpansionBounds(out var lowerBound, out var upperBound);

            for (var stringBuilderLength = lowerBound; stringBuilderLength <= upperBound; stringBuilderLength++)
            {
                IStringBuilder finalStringBuilder = StringBuilder.Empty;
                finalStringBuilder = finalStringBuilder.ConcatWith(singleStringBuilder, stringBuilderLength);
                
                returnList.Add(finalStringBuilder);
            }

            return returnList;
        }

        public override IExpression GetInverse()
        {
            return _maxInverseLength <= 1 
                ? (IExpression) GetComplement() 
                : GetAllCharacterClassInversesUpToMaxInverseLength();
        }

        public override IExpression Clone()
        {
            var clone = new CharacterClassExpression(_alphabet, _random, _maxInverseLength)
            {
                RepeatRange = RepeatRange?.Clone(),
                ExpansionLength = ExpansionLength
            };
            
            clone.AddCharacters(_characterList);
            
            return clone;
        }

        protected override IntegerInterval GetMaxExpansionInterval() =>
            new IntegerInterval(1);

        public CharacterClassExpression GetComplement()
        {
            var complementCharacters = _alphabet.GetAllCharacters().Where(x => !_literals.ContainsKey(x)).ToList();
            var complement = new CharacterClassExpression(_alphabet, _random, _maxInverseLength);
            
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

        public CharacterClassExpression GetIntersection(CharacterClassExpression charClass)
        {
            if(charClass == null || !charClass._characterList.Any() || !_characterList.Any())
                return new CharacterClassExpression(_alphabet, _random, _maxInverseLength);
            
            ClassifyListsByLength(this, charClass, out var shortDict, out var longDict);

            var intersectList = shortDict.Where(x => longDict.ContainsKey(x.Key))
                                         .Select(y => y.Key)
                                         .ToList();
            
            var intersection = new CharacterClassExpression(_alphabet, _random, _maxInverseLength)
            {
                RepeatRange = RepeatRange
            };
            intersection.AddCharacters(intersectList);
            
            return intersection;            
        }

        public CharacterClassExpression GetUnion(CharacterClassExpression charClass)
        {
            var union = new CharacterClassExpression(_alphabet, _random, _maxInverseLength)
            {
                RepeatRange = RepeatRange
            };
            
            union.AddCharacters(_characterList);
    
            if(charClass != null)
                union.AddCharacters(charClass._characterList);
            
            return union;
        }
        
        public void AddCharacters(IEnumerable<string> characters)
        {
            foreach (var letter in characters)
                _literals[letter] = null;

            _characterList = _literals.Keys.ToList();
        }
        
        protected override IStringBuilder ToSingleStringBuilder() =>
            new StringBuilder(new List<CharacterClassExpression> {this});
        
        private UnionExpression GetAllCharacterClassInversesUpToMaxInverseLength()
        {
            var minimalLengthTwoRange = new IntegerInterval();
            minimalLengthTwoRange.TrySetValue(2, _maxInverseLength);
            
            var atLeastLengthTwoWords = new CharacterClassExpression(_alphabet, _random, _maxInverseLength){ RepeatRange = minimalLengthTwoRange };
            atLeastLengthTwoWords.AddCharacters(_alphabet.GetAllCharacters());

            var inverseExpressions = new List<IExpression>
            {
                GetComplement(), 
                atLeastLengthTwoWords
            };
            
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