using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class BasicCollectionGenericPathMappingTests : BasicTestsBase
    {
        #region Basic Types Paths In Generic List Test Cases

        [TestMethod]
        public void MapPathToIntegerInListCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicListElement<BasicElement<int>>>(new[] { "Collection.Element" });
            var objectToMap = new BasicListElement<BasicElement<int>>
                {
                    Collection =
                        new List<BasicElement<int>>
                            {
                                new BasicElement<int> {Element = 1}, new BasicElement<int> {Element = 2},
                                new BasicElement<int> {Element = 3}, new BasicElement<int> {Element = 4},
                                new BasicElement<int> {Element = 5}
                            }
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        [TestMethod]
        public void MapPathToStringInListCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicListElement<BasicElement<string>>>(new[] { "Collection.Element" });
            var objectToMap = new BasicListElement<BasicElement<string>>
                {
                    Collection =
                        new List<BasicElement<string>>
                            {
                                new BasicElement<string> {Element = "1"},
                                new BasicElement<string> {Element = "2"},
                                new BasicElement<string> {Element = "3"},
                                new BasicElement<string> {Element = "4"},
                                new BasicElement<string> {Element = "5"}
                            }
                };

            // Act
            var mapping = mapper.Map(new object[]{objectToMap});

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        #endregion Basic Types Paths In List Test Cases

        #region Basic Types Paths In Array Test Cases

        [TestMethod]
        public void MapPathToIntegerInArrayCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicArrayElement<BasicElement<int>>>(new[] { "Collection.Element" });
            var objectToMap = new BasicArrayElement<BasicElement<int>>
                {
                    Collection =
                        new[]
                            {
                                new BasicElement<int> {Element = 1}, new BasicElement<int> {Element = 2},
                                new BasicElement<int> {Element = 3}, new BasicElement<int> {Element = 4},
                                new BasicElement<int> {Element = 5}
                            }
                };

            // Act
            var mapping = mapper.Map(new object[]{objectToMap});

            // Assert
            Assert.AreEqual(5, GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        [TestMethod]
        public void MapPathToStringInArrayCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicArrayElement<BasicElement<string>>>(new[] { "Collection.Element" });

            var objectToMap = new BasicArrayElement<BasicElement<string>>
                {
                    Collection =
                        new[]
                            {
                                new BasicElement<string> {Element = "1"},
                                new BasicElement<string> {Element = "2"},
                                new BasicElement<string> {Element = "3"},
                                new BasicElement<string> {Element = "4"},
                                new BasicElement<string> {Element = "5"}
                            }
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        #endregion Basic Types Paths In Array Test Cases

        #region Basic Types Paths In Linked List Test Cases

        [TestMethod]
        public void MapPathToStringInLinkedListCollectionOfGenericElements()
        {
            // Arrange
            var mapper = CreateMapper<BasicListElement<BasicElement<string>>>(new[] { "Collection.Element" });
            var objectToMap = new BasicListElement<BasicElement<string>>
                {
                    Collection =
                        new[]
                            {
                                new BasicElement<string> {Element = "1"}, new BasicElement<string> {Element = "2"},
                                new BasicElement<string> {Element = "3"}, new BasicElement<string> {Element = "4"},
                                new BasicElement<string> {Element = "5"}
                            }
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual("5", GetMappedPathFromCollection(mapping, 4, "Collection.Element"));
        }

        #endregion Basic Types Paths In Linked List Test Cases
    }
}
