using NUnit.Framework;
using Regseed.Common.Streams;

namespace Regseed.Test.Common.Streams
{
    [TestFixture]
    public class StringStreamTest
    {
        [Test]
        public void Pop_ReturnsFirstElement()
        {
            var stream = new StringStream("abs");

            var result1 = stream.Pop();
            var result2 = stream.Pop();
            var result3 = stream.Pop();
            
            Assert.AreEqual("a", result1);
            Assert.AreEqual("b", result2);
            Assert.AreEqual("s", result3);
        }
        
        [Test]
        public void Pop_IncrementsCurrentPositionByOne_WhenAllStreamElementsAreCharacters()
        {
            var stream = new StringStream("abs");

            stream.Pop();
            stream.Pop();
            var result = stream.CurrentPosition;
            
            Assert.AreEqual(2, result);
        }
        
        [Test]
        public void Pop_IncrementsCurrentPositionByLengthOfStreamElements_WhenStreamElementsAreLongerThanOne()
        {
            var stream = new StringStream();
            stream.Append("as").Append("asd");

            stream.Pop();
            stream.Pop();
            var result = stream.CurrentPosition;
            
            Assert.AreEqual(5, result);
        }
    }
}