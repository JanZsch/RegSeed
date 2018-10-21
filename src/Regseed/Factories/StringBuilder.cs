using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Expressions;

namespace Regseed.Factories
{
    internal class StringBuilder : IStringBuilder
    {
        private readonly List<CharacterClassExpression> _characterClasses;

        public StringBuilder(List<CharacterClassExpression> characterClasses)
        {
            _characterClasses = characterClasses;
        }

        public static StringBuilder Empty => new StringBuilder(new List<CharacterClassExpression>());

        public virtual string GenerateString()
        {
            return _characterClasses.Aggregate(string.Empty, (current, characterClass) => $"{current}{characterClass.GetCharacter()}");
        }

        public virtual IStringBuilder ConcatWith(IStringBuilder stringBuilder)
        {
            var builder = stringBuilder as StringBuilder ?? throw new ArgumentException();
            
            var list = new List<CharacterClassExpression>();
            list.AddRange(_characterClasses);
            list.AddRange(builder._characterClasses);
            
            return new StringBuilder(list);
        }

        public virtual IStringBuilder IntersectWith(IStringBuilder builder)
        {
            List<CharacterClassExpression> CharacterClassIntersection(IList<CharacterClassExpression> longList, IList<CharacterClassExpression> shortList)
            {
                return shortList.Select((t, i) => t.GetIntersection(longList[i])).ToList();
            }
            
            return ApplyMergeOrIntersectOperation(this, builder, CharacterClassIntersection);
        }

        public virtual IStringBuilder MergeWith(IStringBuilder builder)
        {           
            List<CharacterClassExpression> CharacterClassMerge(IList<CharacterClassExpression> longList, IList<CharacterClassExpression> shortList)
            {
                return shortList.Select((t, i) => longList[i].GetUnion(t)).ToList();
            }

            return ApplyMergeOrIntersectOperation(this, builder, CharacterClassMerge);
        }

        private static IStringBuilder ApplyMergeOrIntersectOperation(IStringBuilder builder1, IStringBuilder builder2, Func<IList<CharacterClassExpression>, IList<CharacterClassExpression>, List<CharacterClassExpression>> operation)
        {
            var characters1 = (builder1 as StringBuilder)?._characterClasses ?? throw new ArgumentException();
            var characters2 = (builder2 as StringBuilder)?._characterClasses ?? throw new ArgumentException();

            return characters1.Count != characters2.Count 
                ? Empty 
                : new StringBuilder(operation(characters1, characters2));
        }
    }
}