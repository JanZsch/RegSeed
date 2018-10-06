using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Results;
using Regseed.Parser;

namespace Regseed.Common.Ranges
{
    public class CharacterRange : BaseRange<string>
    {
        public CharacterRange() : base(null, null)
        {
        }

        public CharacterRange(IList<string> rangeLetters) : base(null, null)
        {
            Characters = rangeLetters ?? throw new ArgumentNullException();
            Start = rangeLetters.FirstOrDefault();
            End = rangeLetters.LastOrDefault();
        }

        public IList<string> Characters { get; private set; }

        public IResult TrySetRange(string start, string end, IParserAlphabet alphabet)
        {
            if(string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                return new FailureResult();
            
            if(!alphabet.TryGetRange(start, end, out var range).IsSuccess)
                return new FailureResult();

            Characters = range;
            return new SuccessResult();
        }
    }
}