using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Parser;
using Regseed.Common.Random;
using Regseed.RegexSeeder.Resources;

namespace Regseed.RegexSeeder.Expressions
{
    internal class CharacterClassExpression : BaseExpression
    {
        private readonly IParserAlphabet _alphabet;
        private readonly List<string> _literalList;
        private readonly IDictionary<string, string> _literals = new Dictionary<string, string>();

        protected CharacterClassExpression() : base(null)
        {
        }

        public CharacterClassExpression(IList<string> letters, IParserAlphabet alphabet, IRandomGenerator random) : base(random)
        {
            if (letters == null || alphabet == null)
                throw new ArgumentNullException();

            var invalidLetter = letters.FirstOrDefault(x => !alphabet.IsValid(x));

            if (invalidLetter != null)
                throw new ArgumentException(string.Format(InterpreterMessages.UnknownLetter, invalidLetter));

            foreach (var letter in letters)
                _literals[letter] = null;

            _literalList = _literals.Keys.ToList();
            _alphabet = alphabet;
        }

        public override IExpression GetComplement()
        {
            var complementLetters = _alphabet.GetAllCharacters().Where(x => !_literals.ContainsKey(x)).ToList();
            return new CharacterClassExpression(complementLetters, _alphabet, _random)
            {
                RepeatRange = RepeatRange
            };
        }

        protected override string ToSingleRegexString()
        {
            if (_literalList.Count == 0)
                return string.Empty;

            return _literalList.Count == 1
                ? _literalList[0]
                : _literalList[_random.GetNextInteger(0, _literalList.Count)];
        }
    }
}