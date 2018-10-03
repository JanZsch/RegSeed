using System;
using NUnit.Framework;
using Regseed.Common.Token;

namespace Regseed.Test.Common.Token
{
    [TestFixture]
    public class BaseTokenTest : BaseToken<string>
    {
        public BaseTokenTest() : base("test", 0, 0)
        {
        }

        public override TEnum GetType<TEnum>() => default(TEnum);

        [Test]
        public void GetValue_ThrowsTypeAccessException_WhenCalledWithWrongGenericType()
        {
            Assert.Throws<TypeAccessException>(() => GetValue<int>());
        }

        [Test]
        public void GetValue_DoesNotThrow_WhenCalledWithMatchingGenericType()
        {
            Assert.DoesNotThrow(() => GetValue<string>());
        }

        [Test]
        public void GetValue_ReturnsFirstConstructorArgument()
        {
            var result = GetValue<string>();

            Assert.AreEqual("test", result);
        }
    }
}