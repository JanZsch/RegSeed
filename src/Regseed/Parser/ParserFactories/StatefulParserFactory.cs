using Regseed.Parser.PrimitiveParsers;
using Regseed.Parser.TokenParser;

namespace Regseed.Parser.ParserFactories
{
    public class StatefulParserFactory : IParserFactory
    {
        private readonly IParserAlphabet _alphabet;

        private bool IsCharacterClassStateActive { get; set; }
        
        public StatefulParserFactory(IParserAlphabet alphabet)
        {
            _alphabet = alphabet;
            IsCharacterClassStateActive = false;
        }

        public bool TryGetTokenParser(string character, out ITokenParser tokenParser)
        {
            tokenParser = null;

            if (!_alphabet.IsValid(character))
                return false;

            CheckCharacterClassStateDeactivation(character);

            if (IsCharacterClassStateActive)
            {
                tokenParser = new CharacterClassCharacterParser(new PrimitiveParser(_alphabet));
                return true;
            }

            CheckCharacterClassStateActivation(character);

            return _alphabet.TryGetTokenParser(character, out tokenParser);
        }

        private void CheckCharacterClassStateActivation(string character)
        {
            if (character.Equals(Resources.SpecialCharacters.OpenCharacterClass) && !IsCharacterClassStateActive)
                IsCharacterClassStateActive = true;
        }

        private void CheckCharacterClassStateDeactivation(string character)
        {
            if (character.Equals(Resources.SpecialCharacters.CloseCharacterClass))
                IsCharacterClassStateActive = false;
        }
    }
}