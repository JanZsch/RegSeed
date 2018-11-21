using System;
using System.Collections.Generic;
using System.Linq;
using Regseed.Common.Builder;

namespace Regseed.Common.Helper
{
    internal static class ExpandHelper
    {
        public static bool WasExpandedStringBuilderListAddedToList(List<IList<IStringBuilder>> stringBuilderList, List<List<IList<IStringBuilder>>> newExpandList, int concatElementPosition)
        {
            var stringBuilders = stringBuilderList[concatElementPosition];
            if (stringBuilders.Count <= 1)
                return false;

            foreach (var stringBuilder in stringBuilders)
            {
                var expandedExpressions = new List<IList<IStringBuilder>>();
                expandedExpressions.AddRange(stringBuilderList.GetRange(0, concatElementPosition));
                expandedExpressions.Add(new List<IStringBuilder> {stringBuilder});
                expandedExpressions.AddRange(stringBuilderList.GetRange(concatElementPosition + 1, stringBuilderList.Count - concatElementPosition - 1));
                newExpandList.Add(expandedExpressions);
            }

            return true;
        }
        
        public static List<List<T>> ExpandListRepresentation<T>(List<List<T>> seed, Func<List<T>, List<List<T>>, int, bool> multiplier)
        {
            var oldCount = 0;
                       
            var expandedUnionList = seed;
            
            while (oldCount != expandedUnionList.Count)
            {
                oldCount = expandedUnionList.Count;
                var newExpandList = new List<List<T>>();

                foreach (var repeatExpressions in expandedUnionList)
                    AddExpressionWithExpandedSubexpression(repeatExpressions, newExpandList, multiplier);

                expandedUnionList = newExpandList;
            }

            return expandedUnionList;
        }

        private static void AddExpressionWithExpandedSubexpression<T>(List<T> repeatExpressions, List<List<T>> newExpandList, Func<List<T>, List<List<T>>, int, bool> multiplier)
        {
            var wasExpanded 
                = repeatExpressions
                    .Where((t, concatElementPosition) => multiplier(repeatExpressions, newExpandList, concatElementPosition))
                    .Any();

            if(!wasExpanded)
                newExpandList.Add(repeatExpressions);
        }        
    }
}