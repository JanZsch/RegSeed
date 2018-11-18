namespace Regseed.Expressions
{
    internal static class ExpressionMetaDataExtension
    {
        public static void UpdateWith(this ExpressionMetaData oldValue, ExpressionMetaData newValue)
        {
            if (oldValue == null || newValue == null)
                return;
            
            oldValue.HasComplement = oldValue.HasComplement || newValue.HasComplement;
            oldValue.HasIntersection = oldValue.HasIntersection || newValue.HasIntersection;
        }
    }
}