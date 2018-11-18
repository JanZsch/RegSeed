using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Results;
using Regseed.Parser.TokenParser;
using Regseed.Resources;

namespace Regseed.Parser
{
    internal class ParserAlphabet : IParserAlphabet
    {
        private readonly HashSet<string> _validCharacterHashSet = new HashSet<string>();
        private readonly List<string> _validCharacterRangeList = new List<string>();

        private readonly IDictionary<string, ITokenParser> _characterTokenParserMapper = new Dictionary<string, ITokenParser>();

        public IParserAlphabet Add(string character, ITokenParser tokenParser, bool isValid = true)
        {
            if (character == null || tokenParser == null)
                throw new ArgumentNullException();

            if (character.Length != 1)
                throw new ArgumentException(RegSeedErrorMessages.AddCharacterTokenParserError);

            if (_characterTokenParserMapper.ContainsKey(character))
                throw new ArgumentException(RegSeedErrorMessages.CharacterTokenParserDuplicate);

            _characterTokenParserMapper.Add(character, tokenParser);

            if (!isValid)
                return this;

            _validCharacterHashSet.Add(character);
            _validCharacterRangeList.Add(character);

            return this;
        }

        public void RemoveCharacter(string character)
        {
            if (_validCharacterRangeList.Contains(character))
                _validCharacterRangeList.Remove(character);

            if (_validCharacterHashSet.Contains(character))
                _validCharacterHashSet.Remove(character);

            if (_characterTokenParserMapper.ContainsKey(character))
                _characterTokenParserMapper.Remove(character);
        }

        public bool TryGetTokenParser(string character, out ITokenParser tokenParser) =>
            _characterTokenParserMapper.TryGetValue(character, out tokenParser);

        public IResult TryGetRange(string startCharacter, string endCharacter, out IList<string> range)
        {
            range = null;
            var startIndex = _validCharacterRangeList.IndexOf(startCharacter);
            var endIndex = _validCharacterRangeList.IndexOf(endCharacter);

            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                return new FailureResult();

            var returnValue = new List<string>(endIndex - startIndex + 1);

            for (var index = startIndex; index <= endIndex; index++)
                returnValue.Add(_validCharacterRangeList[index]);

            range = returnValue;
            return new SuccessResult();
        }

        public IList<string> GetAllCharacters() => 
            _validCharacterRangeList.ToList();

        public bool IsValid(string character) => 
            _validCharacterHashSet.Contains(character);
    }
}