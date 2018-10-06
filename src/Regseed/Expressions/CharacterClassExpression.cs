using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Random;
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

        public override IExpression GetComplement()
        {
            var complementCharacters = _alphabet.GetAllCharacters().Where(x => !_literals.ContainsKey(x)).ToList();
            var complement = new CharacterClassExpression(_alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
            complement.TryAddCharacters(complementCharacters);
            return complement;
        }

        protected override string ToSingleRegexString()
        {
            if (_characterList.Count == 0)
                return string.Empty;

            return _characterList.Count == 1
                ? _characterList[0]
                : _characterList[_random.GetNextInteger(0, _characterList.Count)];
        }
    }
}