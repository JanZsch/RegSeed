using System.Collections.Generic;
using Regseed.Common.Token;
using Regseed.RegexSeeder.RegexTokens;

namespace Regseed.Common.Streams
{
    public sealed class TokenStream : BaseStream<IToken>, ITokenStream
    {
        public TokenStream()
        {
            InitStreamElements();
        }

        public override IToken Pop()
        {
            CurrentPosition++;

            return _streamElements.Dequeue();
        }

        public override IToken LookAhead(long pos)
        {
            return Count == 0 ? new EndOfStreamToken(CurrentPosition) : base.LookAhead(pos);
        }

        protected override void InitStreamElements()
        {
            _streamElements = new Queue<IToken>();
        }
    }
}