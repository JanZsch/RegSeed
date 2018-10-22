using NUnit.Framework;
using Regseed.Expressions;

namespace Regseed.Test.Expressions
{
    [TestFixture]
    public class ExpressionMetaDataExtensionTest
    {
        [Test]
        public void UpdateWith_SetsHasIntersectionToTrue_WhenNewValueIsTrue()
        {
            var oldValue = new ExpressionMetaData();
            var newValue = new ExpressionMetaData {HasIntersection = true};
            
            oldValue.UpdateWith(newValue);
            
            Assert.IsTrue(oldValue.HasIntersection);
        }
        
        [Test]
        public void UpdateWith_LeavesHasIntersectionTrue_WhenOldValueIsTrueButNewValueIsFalse()
        {
            var oldValue = new ExpressionMetaData {HasIntersection = true};
            var newValue = new ExpressionMetaData {HasIntersection = false};
            
            oldValue.UpdateWith(newValue);
            
            Assert.IsTrue(oldValue.HasIntersection);
        }
        
        [Test]
        public void UpdateWith_SetsHasComplementToTrue_WhenNewValueIsTrue()
        {
            var oldValue = new ExpressionMetaData();
            var newValue = new ExpressionMetaData {HasComplement = true};
            
            oldValue.UpdateWith(newValue);
            
            Assert.IsTrue(oldValue.HasIntersection);
        }
        
        [Test]
        public void UpdateWith_LeavesHasComplementTrue_WhenOldValueIsTrueButNewValueIsFalse()
        {
            var oldValue = new ExpressionMetaData {HasComplement = true};
            var newValue = new ExpressionMetaData {HasComplement = false};
            
            oldValue.UpdateWith(newValue);
            
            Assert.IsTrue(oldValue.HasIntersection);
        }
    }
}