using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Resources;
using Regseed.Common.Token;

namespace Regseed.Common.Parser
{
    public class ParserAlphabet : IParserAlphabet
    {
        private readonly HashSet<string> _validLetterHashSet = new HashSet<string>();
        private  List<string> _validLetterList;

        private readonly IDictionary<string, ITokenParser> _letterTokenParserMapper = new Dictionary<string, ITokenParser>();

        public IParserAlphabet Add(string letter, ITokenParser tokenParser, bool isValid = true)
        {
            if (letter == null || tokenParser == null)
                throw new ArgumentNullException();
            
            if (letter.Length != 1)
                throw new ArgumentException(ParserMessages.AddLetterTokenParserError);

            if (!_letterTokenParserMapper.TryAdd(letter, tokenParser))
                throw new ArgumentException(ParserMessages.LetterTokenParserDuplicate);

            if(isValid)
                _validLetterHashSet.Add(letter);

            return this;
        }

        public bool TryGetTokenParser(string letter, out ITokenParser tokenParser)
        {
            tokenParser = null;

            if (!_letterTokenParserMapper.TryGetValue(letter, out var value))
                return false;

            tokenParser = value;
            return true;
        }

        public IList<string> GetRange(string startLetter, string endLetter)
        {
            _validLetterList = _validLetterList ?? _validLetterHashSet.ToList();
            var startIndex = _validLetterList.IndexOf(startLetter);
            var endIndex = _validLetterList.IndexOf(endLetter);

            if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                throw new ArgumentException(string.Format(ParserMessages.InvalidRange, startLetter, endLetter));
            
            var returnValue = new List<string>(endIndex - startIndex + 1);

            for (var index = startIndex; index <= endIndex; index++)
                returnValue.Add(_validLetterList[index]);

            return returnValue;
        }

        public IList<string> GetAllCharacters()
        {
            return _validLetterHashSet.ToList();
        }

        public bool IsValid(string letter)
        {
            return _validLetterHashSet.Contains(letter);
        }
    }
}