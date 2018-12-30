using System.Collections.Generic;
using Regseed.Common.Builder;

namespace Regseed.Common.Helper
{
    internal static class StringBuilderHelper
    {
        public static bool IsStringBuildMergingRequired(IEnumerable<IList<IStringBuilder>> intersectStringBuilders)
        {
            int? generatedStringLength = null;

            foreach (var intersectStringBuilderListRepresentation in intersectStringBuilders)
            foreach (var stringBuilder in intersectStringBuilderListRepresentation)
            {               
                var stringLength = stringBuilder.GeneratedStringLength();

                if (generatedStringLength == null)
                    generatedStringLength = stringLength;

                if (stringLength != generatedStringLength || stringLength == 0)
                    return false;
            }

            return generatedStringLength != null;
        }
    }
}