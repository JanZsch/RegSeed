using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Parser;

namespace Regseed.Common.Ranges
{
    public class CharacterRange : BaseRange<string>
    {
        public CharacterRange(IParserAlphabet alphabet, string start, string end) : base(start, end)
        {
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                throw new ArgumentNullException();

            Letters = alphabet.GetRange(start, end);
        }

        public CharacterRange(IList<string> rangeLetters) : base(null, null)
        {
            Letters = rangeLetters ?? throw new ArgumentNullException();
            Start = rangeLetters.FirstOrDefault();
            End = rangeLetters.LastOrDefault();
        }

        public IList<string> Letters { get; }
    }
}