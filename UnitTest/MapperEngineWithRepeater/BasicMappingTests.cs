using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.MapperEngineWithRepeater
{
    [TestClass]
    public class BasicMappingTests : BasicTestsBase
    {
        #region Basic Mapping Test Cases

        [TestMethod]
        public void MapIntegerInElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicElement<int>>(new[] {"Element"});

            // Act
            var mapping = mapper.Map(new BasicElement<int> {Element = 1});

            // Assert
            Assert.AreEqual(1, GetMappedPath(mapping, "Element"));
        }

        [TestMethod]
        public void MapStringInElement()
        {
            // Arrange
            var mapper = CreateMapper<BasicElement<string>>(new[] { "Element" });

            // Act
            var mapping = mapper.Map(new BasicElement<string> { Element = "1" });

            // Assert
            Assert.AreEqual("1", GetMappedPath(mapping, "Element"));
        }

        #endregion Basic Mapping Test Cases
    }
}
