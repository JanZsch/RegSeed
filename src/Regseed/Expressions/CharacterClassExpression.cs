using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Random;
using Regseed.Factories;
using Regseed.Parser;

namespace Regseed.Expressions
{
    internal class CharacterClassExpression : BaseExpression
    {
        private readonly IParserAlphabet _alphabet;
        private List<string> _characterList = new List<string>();
        private readonly IDictionary<string, string> _literals = new Dictionary<string, string>();

        protected CharacterClassExpression() : base(null)
        {
        }

        public CharacterClassExpression(IParserAlphabet alphabet, IRandomGenerator random) : base(random)
        {
            _alphabet = alphabet ?? throw new ArgumentNullException();
        }

        public bool TryAddCharacters(IList<string> characters)
        {
            if (characters == null)
                return false;
            var invalidLetter = characters.FirstOrDefault(x => !_alphabet.IsValid(x));

            if (invalidLetter != null)
                return false;

            foreach (var letter in characters)
                _literals[letter] = null;

            _characterList = _literals.Keys.ToList();

            return true;
        }

        public override IExpression GetInverse()
        {
            var complementCharacters = _alphabet.GetAllCharacters().Where(x => !_literals.ContainsKey(x)).ToList();
            var complement = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            complement.TryAddCharacters(complementCharacters);
            return complement;
        }
       
        public string GetCharacter()
        {
            if (_characterList.Count == 0)
                return string.Empty;
            
            return  _characterList.Count == 1 
            ? _characterList[0]
            :_characterList[_random.GetNextInteger(0, _characterList.Count)];
        }

        public CharacterClassExpression GetIntersection(CharacterClassExpression charClass)
        {
            if(charClass == null || !charClass._characterList.Any() || !_characterList.Any())
                return new CharacterClassExpression(_alphabet, _random);
            
            IDictionary<string, string> shortDict;
            IDictionary<string, string> longDict;
            
            if (_literals.Count < charClass._literals.Count)
            {
                shortDict = _literals;
                longDict = charClass._literals;
            }
            else
            {
                shortDict = charClass._literals;
                longDict = _literals;
            }

            var intersectList = (from charEntry 
                                 in shortDict 
                                 where longDict.ContainsKey(charEntry.Key) 
                                 select charEntry.Key).ToList();

            var intersection = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            intersection.TryAddCharacters(intersectList);
            
            return intersection;            
        }

        public CharacterClassExpression GetUnion(CharacterClassExpression charClass)
        {
            var union = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            
            union.TryAddCharacters(_characterList);
    
            if(charClass != null)
                union.TryAddCharacters(charClass._characterList);
            
            return union;
        }
        
        protected override IStringBuilder ToSingleStringBuilder() =>
            new StringBuilder(new List<CharacterClassExpression> {this});
    }
}