using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Regseed.Common.Builder;

namespace Regseed.Common.Helper
{
    internal static class ExpandHelper
    {
        public static ExpansionStatus WasExpandedStringBuilderListAddedToList(List<IList<IStringBuilder>> stringBuilderList, List<List<IList<IStringBuilder>>> newExpandList, int concatElementPosition)
        {
            var stringBuilders = stringBuilderList[concatElementPosition];
            if (stringBuilders.Count <= 1)
                return ExpansionStatus.NotExpanded;

            var addGuard = new object();

            Parallel.ForEach(stringBuilders, stringBuilder =>
            {
                var expandedExpressions = new List<IList<IStringBuilder>>();
                expandedExpressions.AddRange(stringBuilderList.GetRange(0, concatElementPosition));
                expandedExpressions.Add(new List<IStringBuilder> {stringBuilder});
                expandedExpressions.AddRange(stringBuilderList.GetRange(concatElementPosition + 1, stringBuilderList.Count - concatElementPosition - 1));

                lock (addGuard)
                    newExpandList.Add(expandedExpressions);
            });
            
            return ExpansionStatus.Expanded;
        }
        
        public static List<List<T>> ExpandListRepresentation<T>(List<List<T>> seed, int? maxLength, Func<List<T>, List<List<T>>, int, ExpansionStatus> multiplier)
        {
            var oldCount = 0;
                       
            var expandedUnionList = seed;
            var wasExpandedUnionListTrimmed = false;
            
            while (oldCount != expandedUnionList.Count || wasExpandedUnionListTrimmed)
            {
                wasExpandedUnionListTrimmed = false;
                oldCount = expandedUnionList.Count;
                var newExpandList = new List<List<T>>();

                foreach (var repeatExpressions in expandedUnionList)
                    AddExpressionWithExpandedSubexpression(repeatExpressions, newExpandList, multiplier);

                expandedUnionList =  RemoveExpressionsExceedingMaxExpansionLength(newExpandList, maxLength);

                if (expandedUnionList.Count != newExpandList.Count)
                    wasExpandedUnionListTrimmed = true;
            }

            return expandedUnionList;
        }

        private static void AddExpressionWithExpandedSubexpression<T>(List<T> repeatExpressions, List<List<T>> newExpandList, Func<List<T>, List<List<T>>, int, ExpansionStatus> multiplier)
        {
            var expansionResults
                = repeatExpressions
                    .Where((t, concatElementPosition) =>
                        multiplier(repeatExpressions, newExpandList, concatElementPosition) == ExpansionStatus.Expanded);

            if (!expansionResults.Any())
                newExpandList.Add(repeatExpressions);
        }

        private static List<List<T>> RemoveExpressionsExceedingMaxExpansionLength<T>(List<List<T>> newExpandList, int? maxLength) =>
            maxLength == null ? newExpandList : newExpandList.Where(x => x.Count <= maxLength).ToList();
    }
}