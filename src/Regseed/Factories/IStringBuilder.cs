using System.Collections.Generic;
using Regseed.Expressions;

namespace Regseed.Factories
{
    public interface IStringBuilder
    {
        IList<CharacterClassExpression> CharacterClasses { get; }

        string GenerateString();
        IStringBuilder ConcatWith(IStringBuilder builder);
        IStringBuilder IntersectWith(IStringBuilder builder);
        IStringBuilder MergeWith(IStringBuilder builder);        
        IStringBuilder Inverse();        
    }
}