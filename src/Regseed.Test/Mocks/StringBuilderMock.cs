using System.Collections.Generic;
using NSubstitute;
using Regseed.Expressions;
using Regseed.Factories;

namespace Regseed.Test.Mocks
{
    internal class StringBuilderMock : StringBuilder
    {
        private readonly IStringBuilder _stringBuilder;

        public StringBuilderMock(List<CharacterClassExpression> characterClasses) : base(characterClasses)
        {
            _stringBuilder = Substitute.For<IStringBuilder>();
        }

        public override string GenerateString()
        {
            return _stringBuilder.GenerateString();
        }

        public override IStringBuilder ConcatWith(IStringBuilder stringBuilder)
        {
            return _stringBuilder;
        }

        public override IStringBuilder IntersectWith(IStringBuilder builder)
        {
            return _stringBuilder;
        }

        public override IStringBuilder MergeWith(IStringBuilder builder)
        {
            return _stringBuilder;
        }
    }
}