using System;
using System.Collections.Generic;
using FlatDTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class ComplexPropertyDescriptorTests : BasicTestsBase
    {
        #region Private Constants

        private const string ComplexObjectCustomDescription = "complex object";
        private const string BasicElementWithComplexObjectCustomDescription = "basic element";

        #endregion Private Constants

        #region Public Helper Classes

        public class ComplexObject
        { }

        public class ComplexObjectHolder
        {
            public IList<BasicElement<ComplexObject>> Collection { get; set; }

            public string String { get; set; }

            public int Int { get; set; }
        }

        public class DeepStructure
        {
            public ComplexObjectHolder ComplexObjectHolder { get; set; }
        }

        public class ComplexObjectMapper : IComplexObjectDescriptor
        {
            public bool HandlesType(Type type)
            {
                return type == typeof (ComplexObject);
            }

            public string Describe(object @object)
            {
                if (!(@object is ComplexObject)) throw new InvalidOperationException();
                return ComplexObjectCustomDescription;
            }
        }

        public class BasicElementMapper : IComplexObjectDescriptor
        {
            public bool HandlesType(Type type)
            {
                return type == typeof(BasicElement<ComplexObject>);
            }

            public string Describe(object @object)
            {
                if (!(@object is BasicElement<ComplexObject>)) throw new InvalidOperationException();
                return BasicElementWithComplexObjectCustomDescription;
            }
        }

        #endregion Public Helper Classes

        #region Private Helper Methods

        void AssertProperMapping(object mapping)
        {
            Assert.AreEqual("String", GetMappedPathFromCollection(mapping, 0, "String"));
            Assert.AreEqual(567, GetMappedPathFromCollection(mapping, 0, "Int"));
            Assert.AreEqual(ComplexObjectCustomDescription, GetMappedPathFromCollection(mapping, 0, "Collection.Element"));

            Assert.AreEqual("String", GetMappedPathFromCollection(mapping, 1, "String"));
            Assert.AreEqual(567, GetMappedPathFromCollection(mapping, 1, "Int"));
            Assert.AreEqual(ComplexObjectCustomDescription, GetMappedPathFromCollection(mapping, 1, "Collection.Element"));
        }

        void AssertCollectionMapping(object mapping)
        {
            Assert.AreEqual(BasicElementWithComplexObjectCustomDescription,
                            GetMappedPathFromCollection(mapping, 0, "Collection"));
            Assert.AreEqual(BasicElementWithComplexObjectCustomDescription,
                            GetMappedPathFromCollection(mapping, 1, "Collection"));
        }

        #endregion Private Helper Methods

        #region Test Cases

        [TestMethod]
        public void ShouldProperlyMapMultipleComplexObjects()
        {
            // Arrange
            var mapper = CreateMapper<ComplexObjectHolder>(new[]
                {
                    "Collection.Element",
                    "String",
                    "Int"
                }, new IComplexObjectDescriptor[] {new ComplexObjectMapper()});
            var objectToMap = new ComplexObjectHolder
                {
                    Collection = new List<BasicElement<ComplexObject>>
                        {
                            new BasicElement<ComplexObject>
                                {
                                    Element = new ComplexObject()
                                },
                            new BasicElement<ComplexObject>
                                {
                                    Element = new ComplexObject()
                                }
                        },
                    String = "String",
                    Int = 567
                };

            // Act
            var mapping = mapper.Map(new object[] {objectToMap});

            // Assert
            AssertProperMapping(mapping);
        }

        [TestMethod]
        public void ShouldProperlyMapCollectionOfComplexObjects()
        {
            // Arrange
            var mapper = CreateMapper<ComplexObjectHolder>(new[]
                {
                    "Collection.Element",
                    "Collection",
                    "String",
                    "Int"
                }, new IComplexObjectDescriptor[] {new ComplexObjectMapper(), new BasicElementMapper()});
            var objectToMap = new ComplexObjectHolder
            {
                Collection = new List<BasicElement<ComplexObject>>
                        {
                            new BasicElement<ComplexObject>
                                {
                                    Element = new ComplexObject()
                                },
                            new BasicElement<ComplexObject>
                                {
                                    Element = new ComplexObject()
                                }
                        },
                String = "String",
                Int = 567
            };

            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            AssertProperMapping(mapping);
            AssertCollectionMapping(mapping);
        }

        [TestMethod]
        public void ShouldProperlyMapDeepStructureWithCollectionOfComplexObjects()
        {
            // Arrange
            var mapper = CreateMapper<DeepStructure>(new[]
                {
                    "ComplexObjectHolder.Collection.Element",
                    "ComplexObjectHolder.Collection",
                    "ComplexObjectHolder.String",
                    "ComplexObjectHolder.Int"
                }, new IComplexObjectDescriptor[] { new ComplexObjectMapper(), new BasicElementMapper() });
            var objectToMap = new DeepStructure
                {
                    ComplexObjectHolder = new ComplexObjectHolder
                        {
                            Collection = new List<BasicElement<ComplexObject>>
                                {
                                    new BasicElement<ComplexObject>
                                        {
                                            Element = new ComplexObject()
                                        },
                                    new BasicElement<ComplexObject>
                                        {
                                            Element = new ComplexObject()
                                        }
                                },
                            String = "String",
                            Int = 567
                        }
                };

            // Act
            var mapping = mapper.Map(new object[] { objectToMap });

            // Assert
            Assert.AreEqual(BasicElementWithComplexObjectCustomDescription,
                            GetMappedPathFromCollection(mapping, 1, "ComplexObjectHolder.Collection"));
        }

        #endregion Test Cases
    }
}
