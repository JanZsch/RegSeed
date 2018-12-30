using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Expressions;
using Regseed.Resources;

namespace Regseed.Common.Builder
{
    internal class StringBuilder : IStringBuilder
    {
        private readonly List<CharacterClassExpression> _characterClasses;

        public StringBuilder(List<CharacterClassExpression> characterClasses)
        {
            _characterClasses = characterClasses;
        }

        public static StringBuilder Empty => 
            new StringBuilder(new List<CharacterClassExpression>());

        public int GeneratedStringLength() => 
            _characterClasses.All(x => x.GetCharacterCount() > 0) ? _characterClasses.Count : 0;
        
        public virtual string GenerateString()
        {
            return GeneratedStringLength() == 0
                ? string.Empty
                : _characterClasses.Aggregate(string.Empty,(current, characterClass) => $"{current}{characterClass.GetRandomCharacter()}");
        }

        public virtual IStringBuilder ConcatWith(IStringBuilder stringBuilder, int times = 1)
        {
            if(times < 0)
                throw new ArgumentOutOfRangeException(RegSeedErrorMessages.InvalidNumberOfConcatenations);
                       
            var list = new List<CharacterClassExpression>();
            list.AddRange(_characterClasses);

            if(!(stringBuilder is StringBuilder builder))
                return new StringBuilder(list);
            
            for (var i = 0; i < times; i++)
                list.AddRange(builder._characterClasses);
            
            return new StringBuilder(list);
        }

        public virtual IStringBuilder IntersectWith(IStringBuilder builder)
        {
            var newCharacters = (builder as StringBuilder)?._characterClasses ?? throw new ArgumentException();

            if(_characterClasses.Count != newCharacters.Count)
                return  Empty;

            var intersectionList = new List<CharacterClassExpression>();
            
            for (var i = 0; i < newCharacters.Count; i++)
            {
                var intersection = newCharacters[i].GetIntersection(_characterClasses[i]);

                if(intersection.GetCharacterCount() == 0)
                    return Empty;
                
                intersectionList.Add(intersection);
            }
            
            return new StringBuilder(intersectionList);
        }
    }
}