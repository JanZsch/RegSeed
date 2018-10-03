using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Resources;

namespace Regseed.Common.Streams
{
    public abstract class BaseStream<TValue> : IStream<TValue>
    {
        public long Count => _streamElements.Count;
        public long CurrentPosition { get; protected set; }

        protected Queue<TValue> _streamElements = null;
        
        protected BaseStream()
        {
            CurrentPosition = 0;
        }

        protected BaseStream(long initialPosition)
        {
            CurrentPosition = initialPosition;
        }

        public abstract TValue Pop();

        public virtual TValue LookAhead(long pos)
        {
            if (_streamElements == null)
                throw new ArgumentNullException(ParserMessages.StreamInitNotCalled);

            if (pos == 0)
                return _streamElements.Peek();
            
            if (pos > int.MaxValue)
                throw new IndexOutOfRangeException($"The current stream implementation only works with {int.MaxValue} elements.");

            if (pos + 1 > _streamElements.Count || pos < 0)
                throw new IndexOutOfRangeException();

            return _streamElements.ElementAt((int) pos);
        }

        public virtual IStream<TValue> Append(TValue value)
        {
            if (_streamElements == null)
                throw new ArgumentNullException(ParserMessages.StreamInitNotCalled);

            _streamElements.Enqueue(value);

            return this;
        }

        public virtual bool IsEmpty()
        {
            if (_streamElements == null)
                throw new ArgumentNullException(ParserMessages.StreamInitNotCalled);

            return !_streamElements.Any();
        }

        public virtual void Flush()
        {
            if (_streamElements == null)
                throw new ArgumentNullException(ParserMessages.StreamInitNotCalled);

            _streamElements.Clear();
        }

        protected abstract void InitStreamElements();
    }
}