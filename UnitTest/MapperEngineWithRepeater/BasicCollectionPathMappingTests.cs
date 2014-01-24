using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class BasicCollectionPathMappingTests : BasicTestsBase
    {
        #region Basic Types Paths In List Test Cases

        public class ListCollectionOfElementsWithInt
        {
            public IList<ElementWithInt> Collection { get; set; }
        }

        public class ListCollectionOfElementsWithString
        {
            public IList<ElementWithString> Collection { get; set; }
        }

        [TestMethod]
        public void MapPathToIntegerInListCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ListCollectionOfElementsWithInt>(new[] { "Collection.Element" });
            var objectToMap = new ListCollectionOfElementsWithInt
            {
                Collection =
                    new List<ElementWithInt>
                            {
                                new ElementWithInt {Element = 1}, new ElementWithInt {Element = 2},
                                new ElementWithInt {Element = 3}, new ElementWithInt {Element = 4},
                                new ElementWithInt {Element = 5}
                            }
            };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        [TestMethod]
        public void MapPathToStringInListCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ListCollectionOfElementsWithString>(new[] { "Collection.Element" });
            var objectToMap = new ListCollectionOfElementsWithString
            {
                Collection =
                    new List<ElementWithString>
                            {
                                new ElementWithString {Element = "1"},
                                new ElementWithString {Element = "2"},
                                new ElementWithString {Element = "3"},
                                new ElementWithString {Element = "4"},
                                new ElementWithString {Element = "5"}
                            }
            };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        #endregion Basic Types Paths In List Test Cases

        #region Basic Types Paths In Array Test Cases

        public class ArrayCollectionOfElementsWithInt
        {
            public ElementWithInt[] Collection { get; set; }
        }

        public class ArrayCollectionOfElementsWithString
        {
            public ElementWithString[] Collection { get; set; }
        }

        [Ignore]//For now
        [TestMethod]
        public void MapPathToIntegerInArrayCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<ArrayCollectionOfElementsWithInt>(new[] { "Collection.Element" });
            var objectToMap = new ArrayCollectionOfElementsWithInt
            {
                Collection =
                    new[]
                            {
                                new ElementWithInt {Element = 1}, new ElementWithInt {Element = 2},
                                new ElementWithInt {Element = 3}, new ElementWithInt {Element = 4},
                                new ElementWithInt {Element = 5}
                            }
            };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        [Ignore]//For now
        [TestMethod]
        public void MapPathToStringInArrayCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<ArrayCollectionOfElementsWithString>(new[] { "Collection.Element" });
            var objectToMap = new ArrayCollectionOfElementsWithString
            {
                Collection =
                    new[]
                            {
                                new ElementWithString {Element = "1"},
                                new ElementWithString {Element = "2"},
                                new ElementWithString {Element = "3"},
                                new ElementWithString {Element = "4"},
                                new ElementWithString {Element = "5"}
                            }
            };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        #endregion Basic Types Paths In Array Test Cases
    }
}
