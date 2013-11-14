using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.MappingEngine
{
    public class FlatMapperEngineWithRepeater : FlatMapperEngine
    {
        protected override BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            var mapper = new FlatMapper<T>(sourceType, destinationType, properties);

            return mapper;
        }
    }

    public class FlatMapper<T> : BaseClass.DTOMapper<T>
    {
        private Type _sourceType;
        private Type _destinationType;
        private List<Tuple<string, List<PropertyInfoEx>>> _properties;

        public FlatMapper(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            _sourceType = sourceType;
            _destinationType = destinationType;
            _properties = properties;
        }

        public override object Map(object sourceDataObject, object destinationObject)
        {
            var parameter = Expression.Parameter(sourceDataObject.GetType(), "value");
            var result = Expression.Parameter(destinationObject.GetType(), "result");
            var iterationCount = Expression.Parameter(typeof(int), "iterationCount");
            //var variable = Expression.Variable(source.List.GetType());
            var property = Expression.Property(parameter, "List");
            //Expression.Assign(variable, Expression.Property(parameter,"List"));
            var itemCount = Expression.Call(property, property.Type.GetMethod("get_Count"));

            var label = Expression.Label(typeof(int));
            var block = Expression.Block(new[] { result, iterationCount },
                                            Expression.Assign(result, Expression.Constant(1)),
                                            Expression.Loop(
                                                Expression.IfThenElse(
                                                                Expression.LessThan(iterationCount, itemCount),
                                                                Expression.AddAssign(iterationCount, Expression.Constant(1)),
                                                                Expression.Break(label, result)),
                                                           label)
                                                   );
            var action = Expression.Lambda<Func<object, object>>(block, parameter).Compile();


            var outValue = action(sourceDataObject);

            return outValue;
        }
    }
}
