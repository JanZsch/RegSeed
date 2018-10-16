using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Expressions;

namespace Regseed.Factories
{
    internal class StringBuilder : IStringBuilder
    {
        private readonly List<CharacterClassExpression> _characterClasses;
        public IList<CharacterClassExpression> CharacterClasses => _characterClasses.ToList();
        
        public StringBuilder(List<CharacterClassExpression> characterClasses)
        {
            _characterClasses = characterClasses;
        }

        public static StringBuilder Empty => new StringBuilder(new List<CharacterClassExpression>());

        public string GenerateString()
        {
            return _characterClasses.Aggregate(string.Empty, (current, characterClass) => $"{current}{characterClass.GetCharacter()}");
        }

        public IStringBuilder ConcatWith(IStringBuilder builder)
        {
            var list = new List<CharacterClassExpression>();
            list.AddRange(CharacterClasses);
            list.AddRange(builder.CharacterClasses);
            
            return new StringBuilder(list);
        }

        public IStringBuilder IntersectWith(IStringBuilder builder)
        {
            List<CharacterClassExpression> CharacterClassIntersection(IList<CharacterClassExpression> longList, IList<CharacterClassExpression> shortList)
            {
                return shortList.Select((t, i) => t.GetIntersection(longList[i])).ToList();
            }
            
            return ApplyMergeOrIntersectOperation(this, builder, CharacterClassIntersection);
        }

        public IStringBuilder MergeWith(IStringBuilder builder)
        {           
            List<CharacterClassExpression> CharacterClassMerge(IList<CharacterClassExpression> longList, IList<CharacterClassExpression> shortList)
            {
                return shortList.Select((t, i) => longList[i].GetUnion(t)).ToList();
            }

            return ApplyMergeOrIntersectOperation(this, builder, CharacterClassMerge);
        }

        public IStringBuilder Inverse()
        {
            var list = new List<CharacterClassExpression>();
            list.AddRange(_characterClasses.Select(x => x.Inverse()).ToList());
            
            return new StringBuilder(list);
        }

        private static IStringBuilder ApplyMergeOrIntersectOperation(IStringBuilder factory1, IStringBuilder factory2, Func<IList<CharacterClassExpression>, IList<CharacterClassExpression>, List<CharacterClassExpression>> operation)
        {
            var characters1 = factory1.CharacterClasses;
            var characters2 = factory2.CharacterClasses;
            
            IList<CharacterClassExpression> shortList ;
            IList<CharacterClassExpression> longList ;

            if (characters2.Count > characters1.Count)
            {
                shortList = characters1;
                longList = characters2;
            }
            else
            {
                shortList = characters2;
                longList = characters1;
            }

            return new StringBuilder(operation(longList, shortList));
        }
    }
}