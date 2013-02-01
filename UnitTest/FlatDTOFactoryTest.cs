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
        public void RunMapperFlatEntityToFlatDTOWithSimplePropertiesInLevel1()
        {

            var data = Mockup.Data.GetSimpleOneLevelMockupData();

            var factory = new FlatDTO.FlatDTOFactory();

            var propertyString = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();
            
            var dtoList = factory.Create<Mockup.Data.MyDTO>(data.ToArray(), propertyString);

            Assert.IsTrue(dtoList.Count() == 2);

            var value = dtoList[0];
            Assert.IsTrue(((string)value.GetType().GetProperty(propertyString[0]).GetValue(value, null)).Equals(data[0].StringValue, StringComparison.InvariantCultureIgnoreCase), "The string property was not correct");
            Assert.IsTrue(((decimal)value.GetType().GetProperty(propertyString[1]).GetValue(value, null)) == data[0].DecimalValue, "The decimal property was not correct");
            Assert.IsTrue(((int)value.GetType().GetProperty(propertyString[2]).GetValue(value, null)) == data[0].IntegerValue, "The int property was not correct");
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
            factory.Create<Mockup.Data.MyDTO>(data.ToArray(), propertyString);

            //Run 2
            factory.Create<Mockup.Data.MyDTO>(data.ToArray(), propertyString);

            //Run 3
            factory.Create<Mockup.Data.MyDTO>(data.ToArray(), propertyString);

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

    }
}
