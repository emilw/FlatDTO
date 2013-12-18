using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatDTO;

namespace UnitTest
{
    [TestClass]
    public class FlatMapperEngineWithRepeaterTests : FlatDTOFactoryTest
    {

        protected override IMapperEngine GetMappingEngine(IEnumerable<IComplexObjectDescriptor> descriptor = null)
        {
            if (descriptor != null)
                return new FlatDTO.MappingEngine.FlatMapperEngineWithRepeater(descriptor);
            else
                return new FlatDTO.MappingEngine.FlatMapperEngineWithRepeater();
        }

        [TestMethod]
        public void RunMapperComplexEntityToFlatDTOWithComplexPropertiesAndSingleMapListProperty()
        {

            var data = Mockup.Data.GetComplexTwoLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetComplexTwoLevelListMockupDataPropertyStrings().Where(x => x =="ComplexPropertyList.StringValue").ToArray();

            var mapper = factory.Create<Mockup.Data.DTOBaseEmpty>(data[0].GetType(), propertyString, GetMappingEngine());
            var dtoList = mapper.Map(data.ToArray());
            
            Assert.IsTrue(dtoList.Count() == 4);
        }

        [TestMethod]
        public void ComplexObjectDescriptorListTests()
        {
            var data = Mockup.Data.GetComplexTwoLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetComplexTwoLevelListMockupDataPropertyStrings().ToList();

            propertyString.Add("ComplexProperty");
            propertyString.Add("ComplexPropertyList.ComplexProperty");

            var descriptors = new List<IComplexObjectDescriptor>();
            descriptors.Add(new Mockup.Data.ComplexPropertyDescriptor(typeof(Mockup.Data.ComplexPropertyLevel1)));
            descriptors.Add(new Mockup.Data.ComplexPropertyLevel2Descriptor(typeof(Mockup.Data.ComplexPropertyLevel2)));

            var mapper = factory.Create<Mockup.Data.DTOBase>(data[0].GetType(), propertyString.ToArray(), GetMappingEngine(descriptors));
            var dtoList = mapper.Map(data.ToArray());

            Assert.IsTrue(dtoList.Count() == 4);

            var value = dtoList[0];
            var rootType = value.GetType();
            Assert.IsTrue(((string)rootType.GetProperty(propertyString[0]).GetValue(value, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)rootType.GetProperty(propertyString[1]).GetValue(value, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)rootType.GetProperty(propertyString[2]).GetValue(value, null)) == data[0].IntegerValue, "The int property was not correct");
            Assert.IsTrue(((string)rootType.GetProperty(propertyString[7]).GetValue(value, null)).Equals(descriptors.FirstOrDefault().Describe(data[0].ComplexProperty), StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");

            Assert.IsTrue(((string)rootType.GetProperty(propertyString[8].Replace(".", "_")).GetValue(value, null)).Equals(descriptors[1].Describe(data[0].ComplexPropertyList[0].ComplexProperty), StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");

            //Get the complex property
            var propertyPath = propertyString[3].Replace(".", "_");

            var property = rootType.GetProperty(propertyPath);

            Assert.IsTrue(((string)property.GetValue(value, null)).Equals(data[0].ComplexProperty.StringValue, StringComparison.InvariantCultureIgnoreCase), "The value on Complex type on level 1 do not match");

            propertyPath = propertyString[4].Replace(".", "_");

            property = rootType.GetProperty(propertyPath);

            Assert.IsTrue(((string)property.GetValue(value, null)).Equals(data[0].ComplexProperty.ComplexProperty.StringValue, StringComparison.InvariantCultureIgnoreCase), "The value on Complex type on level 2 do not match");
        }

    }
}