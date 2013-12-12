﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlatDTO;
using FlatDTO.BaseClass;
using FlatDTO.MappingEngine;
using UnitTest.Mockup;

namespace UnitTest.MapperEngineWithRepeater
{
    public class BasicTestsBase
    {
        #region Helper Classes

        public class BasicElement<T>
        {
            public T Element { get; set; }
        }

        public class BasicEnumerableElement<T>
        {
            public IEnumerable<T> Collection { get; set; } 
        }

        public class BasicListElement<T>
        {
            public IList<T> Collection { get; set; }
        }

        public class BasicArrayElement<T>
        {
            public T[] Collection { get; set; }
        }

        #endregion Helper Classes

        #region Protected Helper Methods

        protected DTOMapper<Data.DTOBaseEmpty> CreateMapper<T>(string[] paths)
        {
            var factory = new FlatDTOFactory();
            var engine = new FlatMapperEngineWithRepeater();
            var mapper = factory.Create<Data.DTOBaseEmpty>(typeof (T), paths, engine);

            return mapper;
        } 

        protected object GetMappedPath(object mapping, string path)
        {
            var property = mapping.GetType().GetProperty(string.Join("_", path.Split('.')));

            if (property == null)
            {
                return null;
            }

            var mapped = property.GetValue(mapping, null);
            return mapped;
        }

        protected object GetMappedPathFromCollection(object mapping, int index, string path)
        {
            var items = (IEnumerable) mapping;
            var element = items.Cast<object>().ElementAt(index);
            var mapped = GetMappedPath(element, path);

            return mapped;
        }

        #endregion Protected Helper Methods
    }
}
