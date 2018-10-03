using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Streams;
using Regseed.Common.Token;

namespace Regseed.Test.Common.Streams
{
    [TestFixture]
    public class TokenStreamTest
    {
        [TestFixture]
        public class StringStreamTest
        {
            [Test]
            public void Pop_ReturnsFirstElement()
            {
                var token1 = Substitute.For<IToken>();
                var token2 = Substitute.For<IToken>();
                var token3 = Substitute.For<IToken>();

                var stream = new TokenStream();
                stream.Append(token1).Append(token2).Append(token3);

                var result1 = stream.Pop();
                var result2 = stream.Pop();
                var result3 = stream.Pop();

                Assert.AreEqual(token1, result1);
                Assert.AreEqual(token2, result2);
                Assert.AreEqual(token3, result3);
            }

            [Test]
            public void Pop_IncrementsCurrentPositionByOne_WhenAllStreamElementsAreCharacters()
            {
                var stream = new TokenStream();
                stream.Append(Substitute.For<IToken>()).Append(Substitute.For<IToken>());

                stream.Pop();
                stream.Pop();
                var result = stream.CurrentPosition;

                Assert.AreEqual(2, result);
            }
        }
    }
}