using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatDTO;

namespace UnitTest
{
    [TestClass]
    public class FlatDTOFactoryTest
    {

        private IMapperEngine GetMappingEngine()
        {
            return new FlatMapperEngine();
        }

        [TestMethod]
        public void RunMapperFlatEntityToFlatDTOWithSimplePropertiesInLevel0()
        {

            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());

            var dtoList = mapper.Map(data.ToArray());

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

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
            var dto = mapper.Map(data[0]);

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

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
            var dtoList = mapper.Map(data.ToArray());

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

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
            var dtoList = mapper.Map(data.ToArray());

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
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());

            //Run 2
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());

            //Run 3
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());

            Assert.IsTrue(_compileHit == 1, "The compile hit event was not 1");
            Assert.IsTrue(_cacheHit == 2, "The cache event was not fired");

            Assert.IsTrue(factory.RegisteredMappers.Count() == 1, "More then one mapper have been created, should be 1");

        }

        [TestMethod]
        public void VerifyThatCacheWorksWithPolymorficInstruction()
        {
            var data = Mockup.Data.GetComplexTwoLevelMockupDataPolymorfic();
            var propertyStringPoly = Mockup.Data.GetComplexTwoLevelMockupDataPropertyStringsPolymorfic();


            var manualPolymorfic = new List<PolymorficTypeConverterInstruction>();

            manualPolymorfic.Add(new PolymorficTypeConverterInstruction("ComplexProperty", typeof(Mockup.Data.ComplexPropertyLevel12)));

            var factory = new FlatDTO.FlatDTOFactory();

            _cacheHit = 0;
            _compileHit = 0;
            factory.CacheHit += new EventHandler(factory_CacheHit);
            factory.Compiling += new EventHandler(factory_Compiling);

            //Run 1
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyStringPoly, GetMappingEngine(), manualPolymorfic);

            //Run 2
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyStringPoly, GetMappingEngine(), manualPolymorfic);

            var propertyString = Mockup.Data.GetComplexTwoLevelMockupDataPropertyStrings();

            //Run 3
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());

            //Run 4
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyStringPoly, GetMappingEngine(), manualPolymorfic);

            Assert.IsTrue(_compileHit == 2, string.Format("The compile hit event was fired {0}, should be {1}", _compileHit, 2));
            Assert.IsTrue(_cacheHit == 2, string.Format("The cache event was fired {0}, should be {1}", _cacheHit, 2));

            Assert.IsTrue(factory.RegisteredMappers.Count() == 2, "The number of mappers are larger than 2");

        }

        void factory_Compiling(object sender, EventArgs e)
        {

            var mapper = (FlatDTO.BaseClass.DTOMapper<Mockup.Data.DTOBase>)sender;

            Assert.IsNotNull(mapper);

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
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
            var resultBase = GetDiffms(start, DateTime.Now);
            
            start = DateTime.Now;
            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
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

            factory.Create<Mockup.Data.DTOBase>(null, null, GetMappingEngine());

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateMethodPropertyInputEmptyPropertyArray()
        {
            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var propertyString = new string[] { };

            var factory = new FlatDTO.FlatDTOFactory();

            factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine());
        }

        [TestMethod]
        public void CreateMethodPropertyInputEmptyDataArray()
        {
            var data = new object[] { };

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var factory = new FlatDTO.FlatDTOFactory();

            var mapper = factory.Create<Mockup.Data.DTOBase>(typeof(Mockup.Data.FlatDataType), propertyString, GetMappingEngine());
            var dtoList = mapper.Map((object[])data.ToArray());

            Assert.IsTrue(dtoList.Count() == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FlatDTO.Exception.MapperException<Mockup.Data.DTOBase>))]
        public void FlatDTOMappingEngineUnMapNotImplemented()
        {
            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();

            var factory = new FlatDTO.FlatDTOFactory();

            var mapper = factory.Create<Mockup.Data.DTOBase>(typeof(Mockup.Data.FlatDataType), propertyString, GetMappingEngine());
            var dtoList = mapper.Map(data.ToArray());
            mapper.UnMap<Mockup.Data.FlatDataType>(dtoList);

            Assert.IsTrue(dtoList.Count() == 0);
        }


        [TestMethod]
        public void RunMapperComplexEntityToFlatDTOWithComplexPropertiesLevel1And2AndManualPolymorficInstruction()
        {

            var data = Mockup.Data.GetComplexTwoLevelMockupDataPolymorfic();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetComplexTwoLevelMockupDataPropertyStringsPolymorfic();

            var manualPolymorfic = new List<PolymorficTypeConverterInstruction>();

            manualPolymorfic.Add(new PolymorficTypeConverterInstruction("ComplexProperty", typeof(Mockup.Data.ComplexPropertyLevel12)));

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString, GetMappingEngine(), manualPolymorfic);
            var dtoList = mapper.Map((object[])data.ToArray());

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

            //Check the polymorfic string
            propertyPath = propertyString[4].Replace(".", "_");

            property = rootType.GetProperty(propertyPath);

            Assert.IsTrue(((string)property.GetValue(value, null)).Equals(((Mockup.Data.ComplexPropertyLevel12)data[0].ComplexProperty).PolymorficString, StringComparison.InvariantCultureIgnoreCase), "The value on the polymorfic string at Complex type on level 2 do not match");

        }

    }
}
