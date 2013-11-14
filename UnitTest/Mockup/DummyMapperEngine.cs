﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.Mockup
{
    public class DummyMapperFlatEngineExtension : FlatDTO.FlatMapperEngine
    {
        protected override string TypeName
        {
            get
            {
                return base.TypeName + "mjau";
            }
        }

    }


    /// <summary>
    /// This is an dummy example, by overriding the mapper engine it's possible to resolve this
    /// </summary>
    public class DummyMapperEngine : FlatDTO.MapperEngine
    {
        //My custom DTO type specified here
        protected override Type CreateDTOType<T>(Type type, List<Tuple<string, List<FlatDTO.PropertyInfoEx>>> properties)
        {
            return typeof(UnitTest.Mockup.Data.FlatDataType);
        }
        
        //My custom mapper
        protected override FlatDTO.BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<FlatDTO.PropertyInfoEx>>> properties)
        {
            var result = new ObjectListMapper<T>();//base.CreateMapper<T>(sourceType, destinationType, properties);

            result.SourceType = sourceType;
            result.DestinationType = destinationType;

            return result;
        
        }
    }
}
