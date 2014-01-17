using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [Ignore]
    [TestClass]
    public class BasicCollectionMappingTests : BasicTestsBase
    {
        #region Basic Types In List Test Cases

        public class ElementWithIntegerList
        {
            public IList<int> Collection { get; set; }
        }

        public class ElementWithStringList
        {
            public IList<string> Collection { get; set; }
        }

        [TestMethod]
        public void MapIntegersInListCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ElementWithIntegerList>(new[] {"Collection"});

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new ElementWithIntegerList {Collection = new List<int> {1, 2, 3, 4, 5}}
                });

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        [TestMethod]
        public void MapStringsInListCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ElementWithStringList>(new[] {"Collection"});

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new ElementWithStringList {Collection = new List<string> {"1", "2", "3", "4", "5"}}
                });

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        #endregion Basic Types In List Test Cases

        #region Basic Types In Array Test Cases

        public class ElementWithIntegerArray
        {
            public int[] Collection { get; set; }
        }

        public class ElementWithStringArray
        {
            public string[] Collection { get; set; }
        }

        [TestMethod]
        public void MapIntegersInArrayCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ElementWithIntegerArray>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new ElementWithIntegerArray {Collection = new[] {1, 2, 3, 4, 5}}
                });

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        [TestMethod]
        public void MapStringsInArrayCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<ElementWithStringArray>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new ElementWithStringArray {Collection = new[] {"1", "2", "3", "4", "5"}}
                });

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        #endregion Basic Types In List Test Cases
    }
}
