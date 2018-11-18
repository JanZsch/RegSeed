using System.Collections.Generic;

namespace Regseed.Streams
{
    internal sealed class StringStream : BaseStream<string>, IStringStream
    {
        private readonly string _input;

        public StringStream(string input = null)
        {
            _input = input;
            InitStreamElements();
        }

        public override string Pop()
        {
            var returnValue = _streamElements.Dequeue();
            CurrentPosition += returnValue.Length;

            return returnValue;
        }

        protected override void InitStreamElements()
        {
            _streamElements = new Queue<string>(_input?.Length ?? 0);

            if (_input == null)
                return;

            foreach (var character in _input)
                _streamElements.Enqueue(character.ToString());
        }
    }
}