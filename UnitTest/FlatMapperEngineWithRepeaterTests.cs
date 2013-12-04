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

        protected override IMapperEngine GetMappingEngine()
        {
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

    }
}