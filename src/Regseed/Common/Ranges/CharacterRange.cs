using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Results;
using Regseed.Parser;

namespace Regseed.Common.Ranges
{
    internal class CharacterRange : BaseRange<string>
    {
        public CharacterRange() : base(null, null)
        {
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