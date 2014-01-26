using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.MappingEngine
{
    public class FlatMapperEngineWithRepeater : FlatMapperEngine
    {
        public FlatMapperEngineWithRepeater(bool createNullableEquivalent = true) {
            this.CreateNullableValueType = createNullableEquivalent;
        }
        public FlatMapperEngineWithRepeater(IEnumerable<IComplexObjectDescriptor> descriptors, bool createNullableEquivalent = true)
            : base(descriptors)
        {
            this.CreateNullableValueType = createNullableEquivalent;
        }

        protected override BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            var itemMapper = base.CreateMapper<T>(sourceType, destinationType, properties);
            var mapper = new Mapper.FlatMapperWithRepeater<T>(sourceType, destinationType, properties, itemMapper);

            return mapper;
        }
    }

}
