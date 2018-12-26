using NUnit.Framework;
using Regseed.Common.Ranges;

namespace Regseed.Test.Common.Ranges
{
    [TestFixture]
    public class IntegerIntervalExtensionTest
    {
        [Test]
        public void ToExpansionBounds_SetsUpperAndLowerBoundTo1_WhenCalledOnNull()
        {
            ((IntegerInterval)null).ToExpansionBounds(out var lowerBound, out var upperBound);
            
            Assert.AreEqual(1, lowerBound);
            Assert.AreEqual(1, upperBound);
        }
        
        [Test]
        public void ToExpansionBounds_OverridesAnyInitialValuesOnLowerAndUpperBound_WhenCalledOnInitialisedBounds()
        {
            var lowerBound = 12;
            var upperBound = lowerBound + 1;
            var interval = new IntegerInterval();
            interval.TrySetValue(1, 2);
            
            interval.ToExpansionBounds(out lowerBound, out upperBound);
            
            Assert.AreEqual(1, lowerBound);
            Assert.AreEqual(2, upperBound);
        }
        
        [Test]
        public void ToExpansionBounds_LowerBoundIs3_WhenLowerIntervalBoundIs3()
        {
            var interval = new IntegerInterval();
            interval.TrySetValue(3, 7);
            
            interval.ToExpansionBounds(out var lowerBound, out _);
            
            Assert.AreEqual(3, lowerBound);
        }
        
        [Test]
        public void ToExpansionBounds_UpperBoundIs3_WhenUpperIntervalBoundIs3()
        {
            var interval = new IntegerInterval();
            interval.TrySetValue(1, 3);
            
            interval.ToExpansionBounds(out _, out var upperBound);
            
            Assert.AreEqual(3, upperBound);
        }
    }
}