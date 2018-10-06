using System.Collections.Generic;
using Regseed.Common.Token;

namespace Regseed.Common.Parser
{
    public interface IParserAlphabet
    {
        IParserAlphabet Add(string character, ITokenParser tokenParser, bool isValidLetter = true);
        void RemoveCharacter(string character);
        bool TryGetTokenParser(string letter, out ITokenParser tokenParser);
        IList<string> GetRange(string startLetter, string endLetter);
        IList<string> GetAllCharacters();
        bool IsValid(string letter);
    }
}