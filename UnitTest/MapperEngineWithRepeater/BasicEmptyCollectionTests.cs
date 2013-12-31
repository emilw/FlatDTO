using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class BasicEmptyCollectionTests : BasicTestsBase
    {
        #region Public Helper Classes

        public class ElementWithDecimal
        {
            public decimal Element { get; set; }
        }

        public class ListWithEmptyCollectionAndDecimal
        {
            public decimal FirstLevelElement { get; set; }
            public IList<ElementWithDecimal> Collection { get; set; } 
        }

        #endregion Public Helper Classes

        #region Test Cases

        [TestMethod]
        public void MapFirstLevelElementWhenCollectionIsEmpty() // This test should be fixed - MFP-5998 :)
        {
            // Arrange
            var mapper = CreateMapper<ListWithEmptyCollectionAndDecimal>(new[]
                {
                    "Collection.Element", "FirstLevelElement"
                });
            var objectToMap = new ListWithEmptyCollectionAndDecimal
                {
                    Collection = new List<ElementWithDecimal>(),
                    FirstLevelElement = 1.234m
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual(1.234m, GetMappedPathFromCollection(mapping, 0, "FirstLevelElement"));

            // Expected behavior here is that FirstLevelElement is mapped one time when any other path goes through collection,
            // but elements from empty collection should not be mapped.
            //
            // Currently for empty collection the element not from collection is not mapped, but should be mapped.
        }

        [TestMethod]
        public void MapFirstElementWhenCollectionIsNull() // This test is excessive and there is no need for it to pass.
        {
            // Arrange
            var mapper = CreateMapper<ListWithEmptyCollectionAndDecimal>(new[]
                {
                    "Collection.Element", "FirstLevelElement"
                });
            var objectToMap = new ListWithEmptyCollectionAndDecimal
            {
                FirstLevelElement = 1.234m
            };

            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            Assert.AreEqual(1.234m, GetMappedPathFromCollection(mapping, 0, "FirstLevelElement"));
        }

        [TestMethod]
        public void MapFirstElementWhenCollectionIsNotEmpty() // This test is for reference purposes and should pass.
        {
            // Arrange
            var mapper = CreateMapper<ListWithEmptyCollectionAndDecimal>(new[]
                {
                    "Collection.Element", "FirstLevelElement"
                });
            var objectToMap = new ListWithEmptyCollectionAndDecimal
            {
                Collection = new List<ElementWithDecimal>
                    {
                        new ElementWithDecimal
                            {
                                Element = 4.321m
                            }
                    },
                FirstLevelElement = 1.234m
            };

            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            Assert.AreEqual(1.234m, GetMappedPathFromCollection(mapping, 0, "FirstLevelElement"));
            Assert.AreEqual(4.321m, GetMappedPathFromCollection(mapping, 0, "Collection.Element"));
        }

        #endregion Test Cases
    }
}
