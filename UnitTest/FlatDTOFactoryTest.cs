using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class FlatDTOFactoryTest
    {

        [TestMethod]
        public void RunMapperFlatEntityToFlatDTOWithSimplePropertiesInLevel0()
        {

            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();
            
            var dtoList = factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            Assert.IsTrue(dtoList.Count() == 2);

            var value = dtoList[0];
            Assert.IsTrue(((string)value.GetType().GetProperty(propertyString[0]).GetValue(value, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)value.GetType().GetProperty(propertyString[1]).GetValue(value, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)value.GetType().GetProperty(propertyString[2]).GetValue(value, null)) == data[0].IntegerValue, "The int property was not correct");
        }


        [TestMethod]
        public void RunSingleMapperFlatEntityToFlatDTOWithSimplePropertiesInLevel0()
        {

            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var dto = factory.Create<Mockup.Data.DTOBase>(data[0], propertyString);

            Assert.IsTrue(((string)dto.GetType().GetProperty(propertyString[0]).GetValue(dto, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)dto.GetType().GetProperty(propertyString[1]).GetValue(dto, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)dto.GetType().GetProperty(propertyString[2]).GetValue(dto, null)) == data[0].IntegerValue, "The int property was not correct");
        }

        [TestMethod]
        [ExpectedException(typeof(FlatDTO.Exception.PropertyDoNotExistException))]
        public void RunMapperFlatEntityToFlatDTOWithSimplePropertiesInLevel0WithNonExistingProperty()
        {

            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            propertyString[2] = propertyString[2] + "XYZ";

            var dtoList = factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            Assert.IsTrue(dtoList.Count() == 2);

            var value = dtoList[0];
            Assert.IsTrue(((string)value.GetType().GetProperty(propertyString[0]).GetValue(value, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)value.GetType().GetProperty(propertyString[1]).GetValue(value, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)value.GetType().GetProperty(propertyString[2]).GetValue(value, null)) == data[0].IntegerValue, "The int property was not correct");
        }

        [TestMethod]
        public void RunMapperComplexEntityToFlatDTOWithComplexPropertiesLevel1And2()
        {

            var data = Mockup.Data.GetComplexTwoLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetComplexTwoLevelMockupDataPropertyStrings();

            var dtoList = factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            Assert.IsTrue(dtoList.Count() == 2);

            var value = dtoList[0];
            var rootType = value.GetType();
            Assert.IsTrue(((string)rootType.GetProperty(propertyString[0]).GetValue(value, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)rootType.GetProperty(propertyString[1]).GetValue(value, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)rootType.GetProperty(propertyString[2]).GetValue(value, null)) == data[0].IntegerValue, "The int property was not correct");

            //Get the complex property
            var propertyPath = propertyString[3].Replace(".", "_");

            var property = rootType.GetProperty(propertyPath);

            Assert.IsTrue(((string)property.GetValue(value, null)).Equals(data[0].ComplexProperty.StringValue, StringComparison.InvariantCultureIgnoreCase), "The value on Complex type on level 1 do not match");

            propertyPath = propertyString[4].Replace(".", "_");

            property = rootType.GetProperty(propertyPath);

            Assert.IsTrue(((string)property.GetValue(value, null)).Equals(data[0].ComplexProperty.ComplexProperty.StringValue, StringComparison.InvariantCultureIgnoreCase), "The value on Complex type on level 2 do not match");

        }

        [TestMethod]
        public void VerifyThatCacheWorks()
        {
            var data = Mockup.Data.GetSimpleOneLevelMockupData();
            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var factory = new FlatDTO.FlatDTOFactory();

            _cacheHit = 0;
            _compileHit = 0;
            factory.CacheHit += new EventHandler(factory_CacheHit);
            factory.Compiling += new EventHandler(factory_Compiling);

            //Run 1
            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            //Run 2
            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            //Run 3
            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            Assert.IsTrue(_compileHit == 1, "The cache event was not fired");
            Assert.IsTrue(_cacheHit == 2, "The cache event was not fired");

            Assert.IsTrue(factory.CurrentListOfMappers.Count() == 1, "More then one mapper have been created, should be 1");


        }

        void factory_Compiling(object sender, EventArgs e)
        {
            _compileHit = _compileHit + 1;
        }

        private int _cacheHit = 0;
        private int _compileHit = 0;

        void factory_CacheHit(object sender, EventArgs e)
        {
            _cacheHit = _cacheHit + 1;
        }


        [TestMethod]
        public void TestThatTheCacheIsFasterThenCompiling()
        {
            var data = Mockup.Data.GetSimpleOneLevelMockupData();
            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var factory = new FlatDTO.FlatDTOFactory();

            var start = DateTime.Now;
            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);
            var resultBase = GetDiffms(start, DateTime.Now);
            
            start = DateTime.Now;
            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);
            var result = GetDiffms(start, DateTime.Now);

            Assert.IsTrue(result < resultBase, "The timing cache result is {0}, the compile result is {1}", result, resultBase);
        }

        private long GetDiffms(DateTime start, DateTime stop)
        {
            return (stop.Ticks - start.Ticks) / 10000;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMethodPropertyInputNull()
        {
            var factory = new FlatDTO.FlatDTOFactory();

            factory.Create<Mockup.Data.DTOBase>(null, null);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateMethodPropertyInputEmptyPropertyArray()
        {
            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var propertyString = new string[] { };

            var factory = new FlatDTO.FlatDTOFactory();

            factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);
        }

        [TestMethod]
        public void CreateMethodPropertyInputEmptyDataArray()
        {
            var data = new object[] { };

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var factory = new FlatDTO.FlatDTOFactory();

            var dtoList = factory.Create<Mockup.Data.DTOBase>(data.ToArray(), propertyString);

            Assert.IsTrue(dtoList.Count() == 0);
        }

    }
}
