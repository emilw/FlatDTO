using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class BasicCollectionGenericMappingTests : BasicTestsBase
    {
        #region Basic Types In Generic List Test Cases

        [TestMethod]
        public void MapIntegersInListCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicListElement<int>>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new BasicListElement<int> {Collection = new List<int> {1, 2, 3, 4, 5}}
                });

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        [TestMethod]
        public void MapStringsInListCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicListElement<string>>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new BasicListElement<string> {Collection = new List<string> {"1", "2", "3", "4", "5"}}
                });

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        #endregion Basic Types In List Test Cases

        #region Basic Types In Generic Array Test Cases

        [TestMethod]
        public void MapIntegersInArrayCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicArrayElement<int>>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new BasicArrayElement<int> {Collection = new[] {1, 2, 3, 4, 5}}
                });

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        [TestMethod]
        public void MapStringsInArrayCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicArrayElement<string>>(new[] { "Collection" });

            // Act
            var mapping = mapper.Map(new object[]
                {
                    new BasicArrayElement<string> {Collection = new[] {"1", "2", "3", "4", "5"}}
                });

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        #endregion Basic Types In Generic Array Test Cases

        #region Basic Types In Generic Linked List Test Cases

        [TestMethod]
        public void MapStringsInLinkedListCollectionOfGenericElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicEnumerableElement<string>>(new[] { "Collection" });
            var objectToMap = new BasicEnumerableElement<string>
                {
                    Collection = new LinkedList<string>(new[] {"1", "2", "3", "4", "5"})
                };
            
            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection"));
        }

        #endregion Basic Types In Generic Linked List Test Cases
    }
}
