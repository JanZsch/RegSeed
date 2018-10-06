using System.Collections.Generic;
using Regseed.Common.Results;
using Regseed.Parser.TokenParser;

namespace Regseed.Parser
{
    public interface IParserAlphabet
    {
        IParserAlphabet Add(string character, ITokenParser tokenParser, bool isValidLetter = true);
        void RemoveCharacter(string character);
        bool TryGetTokenParser(string letter, out ITokenParser tokenParser);
        IResult TryGetRange(string startLetter, string endLetter, out IList<string> characterRange);
        IList<string> GetAllCharacters();
        bool IsValid(string letter);
    }
}