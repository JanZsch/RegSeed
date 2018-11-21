using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Regseed.Common.Builder;
using Regseed.Common.Helper;

namespace Regseed.Test.Common.Helper
{
    [TestFixture]
    public class ExpandHelperTest
    {
        [TestCaseSource(nameof(GetWasExpandedStringBuilderListAddedToList))]
        public void WasExpandedStringBuilderListAddedToList_ReturnsFalse_WhenRepresentationListContainsNoSublistToBeExpanded(List<IList<IStringBuilder>> seed)
        {
            var initialExpandedList = new List<List<IList<IStringBuilder>>>{seed};
         
            var result = ExpandHelper.WasExpandedStringBuilderListAddedToList(seed, initialExpandedList, 0);
            
            Assert.IsFalse(result);
            Assert.AreEqual(1, initialExpandedList.Count);
        }

        private static IEnumerable<object[]> GetWasExpandedStringBuilderListAddedToList()
        {
            yield return new object[] {new List<IList<IStringBuilder>> {new List<IStringBuilder>()}};
            yield return new object[] {new List<IList<IStringBuilder>> {new List<IStringBuilder> {Substitute.For<IStringBuilder>()}}};
        }

        [Test]
        public void WasExpandedStringBuilderListAddedToList_ReturnsTrue_WhenRepresentationListContainsSublistToBeExpanded()
        {
            var seed = new List<IList<IStringBuilder>>
            {
                new List<IStringBuilder> {Substitute.For<IStringBuilder>(), Substitute.For<IStringBuilder>()}
            };
            var resultExpandedList = new List<List<IList<IStringBuilder>>>();
         
            var result = ExpandHelper.WasExpandedStringBuilderListAddedToList(seed, resultExpandedList, 0);
            
            Assert.IsTrue(result);
            Assert.AreEqual(2, resultExpandedList.Count);
        }

        [Test]
        public void WasExpandedStringBuilderListAddedToList_InitialExpandListContains3Elements_WhenRepresentationListContainsSublistWith3ElementsToBeExpanded()
        {
            var seed = new List<IList<IStringBuilder>>
            {
                new List<IStringBuilder>{Substitute.For<IStringBuilder>()},
                new List<IStringBuilder> {Substitute.For<IStringBuilder>(), Substitute.For<IStringBuilder>(), Substitute.For<IStringBuilder>()},
                new List<IStringBuilder>{Substitute.For<IStringBuilder>()},
                new List<IStringBuilder>{Substitute.For<IStringBuilder>()}
            };
            var resultExpandedList = new List<List<IList<IStringBuilder>>>();
         
            ExpandHelper.WasExpandedStringBuilderListAddedToList(seed, resultExpandedList, 1);
            
            Assert.AreEqual(3, resultExpandedList.Count);
            Assert.AreEqual(4, resultExpandedList[0].Count);
            Assert.AreEqual(4, resultExpandedList[1].Count);
            Assert.AreEqual(4, resultExpandedList[2].Count);
        }

        [Test]
        public void ExpandListRepresentation_ReturnedListHasOneElement_WhenSeedContainsNoElementToBeExpanded()
        {
            bool WasNotExpanded(List<int> expression, List<List<int>> expandedElements, int pos) => false;
            var seed = new List<List<int>> {new List<int>{1}};

            var result = ExpandHelper.ExpandListRepresentation(seed, WasNotExpanded);
            
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ExpandListRepresentation_ReturnedListHasThreeElements_WhenSeedContainsElementToBeExpandedIntoThreeSubelements()
        {
            bool WasExpanded(List<int> expression, List<List<int>> expandedElements, int pos)
            {
                foreach (var i in expression)
                    expandedElements.Add(new List<int>{i});

                return true;
            }
            var seed = new List<List<int>> {new List<int>{1,2,3}};

            var result = ExpandHelper.ExpandListRepresentation(seed, WasExpanded);
            
            Assert.AreEqual(3, result.Count);
        }
    }
}