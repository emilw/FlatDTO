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

        private const string CustomDescription = "custom description";

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

        public class ComplexObjectMapper : IComplexObjectDescriptor
        {
            public bool HandlesType(Type type)
            {
                return type == typeof (ComplexObject);
            }

            public string Describe(object @object)
            {
                return CustomDescription;
            }
        }

        #endregion Public Helper Classes

        #region Private Helper Methods

        void AssertProperMapping(object mapping)
        {
            Assert.AreEqual("String", GetMappedPathFromCollection(mapping, 0, "String"));
            Assert.AreEqual(567, GetMappedPathFromCollection(mapping, 0, "Int"));
            Assert.AreEqual(CustomDescription, GetMappedPathFromCollection(mapping, 0, "Collection.Element"));

            Assert.AreEqual("String", GetMappedPathFromCollection(mapping, 1, "String"));
            Assert.AreEqual(567, GetMappedPathFromCollection(mapping, 1, "Int"));
            Assert.AreEqual(CustomDescription, GetMappedPathFromCollection(mapping, 1, "Collection.Element"));
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

        #endregion Test Cases
    }
}
