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

    }
}