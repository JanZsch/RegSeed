using System;
using NUnit.Framework;
using Regseed.Parser.RegexTokens;

namespace Regseed.Test.Parser.RegexTokens
{
    [TestFixture]
    internal class BaseTokenTest : BaseToken<string>
    {
        public BaseTokenTest() : base("thunder", 0, 0)
        {
        }

        public override TEnum GetType<TEnum>()
        {
            return default(TEnum);
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

            Assert.AreEqual("thunder", result);
        }

        [Test]
        public void GetValue_ThrowsTypeAccessException_WhenCalledWithWrongGenericType()
        {
            Assert.Throws<TypeAccessException>(() => GetValue<int>());
        }
    }
}