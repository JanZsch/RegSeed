using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Resources;
using Regseed.Common.Token;

namespace Regseed.Common.Parser
{
    public class ParserAlphabet : IParserAlphabet
    {
        private readonly HashSet<string> _validCharacterHashSet = new HashSet<string>();
        private  List<string> _validCharacterRangeList = new List<string>();

        private readonly IDictionary<string, ITokenParser> _characterTokenParserMapper = new Dictionary<string, ITokenParser>();

        public IParserAlphabet Add(string character, ITokenParser tokenParser, bool isValid = true)
        {
            if (character == null || tokenParser == null)
                throw new ArgumentNullException();
            
            if (character.Length != 1)
                throw new ArgumentException(ParserMessages.AddLetterTokenParserError);

            if (!_characterTokenParserMapper.TryAdd(character, tokenParser))
                throw new ArgumentException(ParserMessages.LetterTokenParserDuplicate);

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

        public bool TryGetTokenParser(string letter, out ITokenParser tokenParser)
        {
            tokenParser = null;

            if (!_characterTokenParserMapper.TryGetValue(letter, out var value))
                return false;

            tokenParser = value;
            return true;
        }

        public IList<string> GetRange(string startLetter, string endLetter)
        {
            var startIndex = _validCharacterRangeList.IndexOf(startLetter);
            var endIndex = _validCharacterRangeList.IndexOf(endLetter);
            
            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                throw new ArgumentException(string.Format(ParserMessages.InvalidRange, startLetter, endLetter));
            
            var returnValue = new List<string>(endIndex - startIndex + 1);

            for (var index = startIndex; index <= endIndex; index++)
                returnValue.Add(_validCharacterRangeList[index]);

            return returnValue;
        }

        public IList<string> GetAllCharacters() =>  _validCharacterRangeList.ToList();

        public bool IsValid(string letter) => _validCharacterHashSet.Contains(letter);
    }
}