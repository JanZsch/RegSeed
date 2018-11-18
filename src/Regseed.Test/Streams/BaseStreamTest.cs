using System;
using System.Collections.Generic;
using NUnit.Framework;
using Regseed.Streams;

namespace Regseed.Test.Streams
{
    [TestFixture]
    internal class BaseStreamTest : BaseStream<string>
    {
        [SetUp]
        public void SetUp()
        {
            _streamElements = new Queue<string>();
        }

        public override string Pop()
        {
            CurrentPosition++;
            return _streamElements.Dequeue();
        }

        protected override void InitStreamElements()
        {
        }

        [TestCase(0)]
        [TestCase(3)]
        public void Count_ReturnsN_WhenStreamContainsNElements(int expectedCount)
        {
            for (var i = 0; i < expectedCount; i++)
                Append("s");

            var result = Count;

            Assert.AreEqual(expectedCount, result);
        }

        [TestCase(10)]
        [TestCase(3)]
        public void
            LookAhead_ThrowsArgumentOutOfBoundsException_WhenLookAheadLengthIsLargerThanRemainingNumberOfElements(
                int lookAheadLength)
        {
            Append("a").Append("b").Append("c").Append("d");
            Pop();

            Assert.Throws<IndexOutOfRangeException>(() => LookAhead(lookAheadLength));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void LookAhead_DoesNotThrow_WhenLookAheadLengthIsSmallerOrEqualThanRemainingNumberOfElements(
            int lookAheadLength)
        {
            Append("a").Append("b").Append("c").Append("d");
            Pop();

            Assert.DoesNotThrow(() => LookAhead(lookAheadLength));
        }

        [Test]
        public void Append_AddsElementToStreamAtTheEnd_WhenStreamWasInitialised()
        {
            Append("a").Append("1").Append("Ü");

            var result1 = Pop();
            var result2 = Pop();
            var result3 = Pop();

            Assert.AreEqual("a", result1);
            Assert.AreEqual("1", result2);
            Assert.AreEqual("Ü", result3);
        }

        [Test]
        public void Append_ThrowsArgumentNullException_WhenStreamWasNotInitialised()
        {
            _streamElements = null;

            Assert.Throws<ArgumentNullException>(() => Append("s"));
        }

        [Test]
        public void Flush_EmptiesStream()
        {
            Append("1");

            Flush();
            var result = IsEmpty();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEmpty_ReturnsFalse_WhenStreamIsNotEmpty()
        {
            Append("1");

            var result = IsEmpty();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsEmpty_ReturnsTrue_WhenStreamIsEmpty()
        {
            Append("1");
            Pop();

            var result = IsEmpty();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsEmpty_ThrowsArgumentNullException_WhenStreamWasNotInitialised()
        {
            _streamElements = null;

            Assert.Throws<ArgumentNullException>(() => IsEmpty());
        }

        [Test]
        public void LookAhead_ReturnsC_WhenCurrentPositionIsOneAndSecondElementIsCAndLookAheadLengthIsOne()
        {
            Append("a").Append("b").Append("c").Append("d");
            Pop();

            var result = LookAhead(1);

            Assert.AreEqual("c", result);
        }

        [Test]
        public void LookAhead_ThrowsArgumentNullException_StreamElementsNotInitialised()
        {
            _streamElements = null;

            Assert.Throws<ArgumentNullException>(() => LookAhead(2));
        }
    }
}