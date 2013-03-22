using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatDTO;

namespace UnitTest
{
    [TestClass]
    public class ExtensionTest
    {

        [TestMethod]
        public void DummyMapperExtensionTest()
        {
            var factory = new FlatDTO.FlatDTOFactory();
            var properties = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();
            var data = Mockup.Data.GetSimpleOneLevelMockupData();
            var mapper = factory.Create<Mockup.Data.FlatDataType>(data[0].GetType(), properties, new Mockup.DummyMapperFlatEngineExtension());

            Assert.IsTrue(mapper.DestinationType.FullName.Contains("mjau"));
        }
    }
}
