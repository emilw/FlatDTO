using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class MultipleCollectionPathMappingTests : BasicTestsBase
    {
        #region Private Helper Classes

        public class ElementWithIntAndString
        {
            public int IntElement { get; set; }
            public string StringElement { get; set; }
        }

        public class ListCollectionOfElementsWithIntAndString
        {
            public IList<ElementWithIntAndString> Collection { get; set; } 
        }

        public class DeepStructure
        {
            public ListCollectionOfElementsWithIntAndString ComplexObject { get; set; }
        }

        #endregion Private Helper Classes

        #region Test Cases

        [TestMethod]
        public void MapPathToIntegerAndStringInListCollectionElement()
        {
            // Arrange
            var mapper = CreateMapper<ListCollectionOfElementsWithIntAndString>(new[]
                {
                    "Collection.IntElement", "Collection.StringElement"
                });
            var objectToMap = new ListCollectionOfElementsWithIntAndString
                {
                    Collection = new List<ElementWithIntAndString>
                        {
                            new ElementWithIntAndString
                                {
                                    IntElement = 1,
                                    StringElement = "1"
                                },
                            new ElementWithIntAndString
                                {
                                    IntElement = 2,
                                    StringElement = "2"
                                },
                            new ElementWithIntAndString
                                {
                                    IntElement = 3,
                                    StringElement = "3"
                                }
                        }
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            Assert.AreEqual(3, GetMappedPathFromCollection(mapping, 2, "Collection.IntElement"));
            Assert.AreEqual("2", GetMappedPathFromCollection(mapping, 1, "Collection.StringElement"));
        }

        [TestMethod]
        public void MapPathToCollectionThroughDeepStructure()
        {
            // Arrange
            var mapper = CreateMapper<DeepStructure>(new[]
                {
                    "ComplexObject.Collection.IntElement", "ComplexObject.Collection.StringElement"
                });
            var objectToMap = new DeepStructure
                {
                    ComplexObject = new ListCollectionOfElementsWithIntAndString
                        {
                            Collection = new List<ElementWithIntAndString>
                                {
                                    new ElementWithIntAndString
                                        {
                                            IntElement = 1,
                                            StringElement = "1"
                                        },
                                    new ElementWithIntAndString
                                        {
                                            IntElement = 2,
                                            StringElement = "2"
                                        },
                                    new ElementWithIntAndString
                                        {
                                            IntElement = 3,
                                            StringElement = "3"
                                        }
                                }
                        }
                };

            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            Assert.AreEqual(3, GetMappedPathFromCollection(mapping, 2, "ComplexObject.Collection.IntElement"));
            Assert.AreEqual("2", GetMappedPathFromCollection(mapping, 1, "ComplexObject.Collection.StringElement"));
        }

        #endregion Test Cases
    }
}
