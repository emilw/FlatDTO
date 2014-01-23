using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.MappingEngine
{
    public class FlatMapperEngineWithRepeater : FlatMapperEngine
    {
        public FlatMapperEngineWithRepeater() { }
        public FlatMapperEngineWithRepeater(IEnumerable<IComplexObjectDescriptor> descriptors) : base(descriptors) {}

        protected override BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            this.CreateNullableValueType = true;
            var itemMapper = base.CreateMapper<T>(sourceType, destinationType, properties);
            var mapper = new Mapper.FlatMapperWithRepeater<T>(sourceType, destinationType, properties, itemMapper);

            return mapper;
        }
    }

}
