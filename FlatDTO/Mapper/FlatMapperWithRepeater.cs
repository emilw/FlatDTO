using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.Mapper
{
    public class FlatMapperWithRepeater<T> : BaseClass.DTOMapper<T>
    {
        private List<Tuple<string, List<PropertyInfoEx>>> CollectionProperties;
        private IDTOMapper<T> ItemMapper = null;

        private IEnumerable<object> ItemsToMap = null;
        private Action<object, object> ListMap = null;
        private Func<object, IEnumerable<object>> ItemsToMapFunc = null;

        public FlatMapperWithRepeater(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties, IDTOMapper<T> itemMapper)
        {
            this.ItemMapper = itemMapper;
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            CollectionProperties = properties.Where(x => x.Item2.Exists(y => y.IsCollection)).ToList();
            checkProperties();
        }

        private void checkProperties()
        {
            var collectionPropertyInfo = new List<PropertyInfoEx>();
            CollectionProperties.ForEach(x =>
            {
                collectionPropertyInfo.AddRange(x.Item2.Where(y => y.IsCollection).ToList());
            });

            var uniqueResults = collectionPropertyInfo.GroupBy(x => x.SystemProperty.Name);

            if (uniqueResults.Count() > 1)
                throw new Exception.MultipleListItemInstructionsException(collectionPropertyInfo.Select(x => x.SystemProperty.Name).Distinct().ToArray());
        }

        private Expression getPropertyMapExpression(object item, object destinationObject, Tuple<string, List<PropertyInfoEx>> propertyPath,
                                                    ParameterExpression input, ParameterExpression output)
        {
            var valueLine = Expression.Convert(input, item.GetType());

            Expression valueProperty = valueLine;
            IComplexObjectDescriptor descriptor = null;

            //Get the property from the source, iterate to the end of the list, it's ordered
            foreach (var property in propertyPath.Item2)
            {
                if (!property.IsCollection)
                {
                    valueProperty = Expression.PropertyOrField(valueProperty, property.SystemProperty.Name);
                }
            }

            if (propertyPath.Item2.Last().HasComplexObjectDescriptor)
            {
                descriptor = propertyPath.Item2.Last().ComplexObjectDescriptor;
            }

            if (descriptor != null)
            {

                //var complexDescriptorExpression = Expression.Variable(typeof(IComplexObjectDescriptor));
                //var descriptorExpression = Expression.Assign(complexDescriptorExpression, Expression.Constant(descriptor));
                //var describeString = Expression.Call(descriptorExpression, "Describe", new Expression[] {valueProperty});
                //var describeString = Expression.Call(descriptorExpression,typeof(IComplexObjectDescriptor).GetMethod("Describe"), new Expression[] { valueProperty });
                var describeString = Expression.Call(Expression.Constant(descriptor), typeof(IComplexObjectDescriptor).GetMethod("Describe"), new Expression[] { valueProperty });

                //Get the real type object to map against
                var result = Expression.Convert(output, destinationObject.GetType());
                //Get the property to assign
                var resultProperty = Expression.Property(result, propertyPath.Item1);

                //Assign the property from the source property to the result property
                var assignExpression = Expression.Assign(resultProperty, describeString);

                return assignExpression;
            }
            else
            {

                //Get the real type object to map against
                var result = Expression.Convert(output, destinationObject.GetType());
                //Get the property to assign
                var resultProperty = Expression.Property(result, propertyPath.Item1);

                //Assign the property from the source property to the result property
                var assignExpression = Expression.Assign(resultProperty, valueProperty);

                return assignExpression;
            }
        }

        public override object Map(object sourceDataObject, object destinationObject)
        {
            if (CollectionProperties.Count == 0)
            {
                return ItemMapper.Map(sourceDataObject, destinationObject);
            }
            else
            {
                try
                {
                    List<T> list = new List<T>();

                    var prop = CollectionProperties.FirstOrDefault();

                    var itemsToMap = GetItemsToMap(sourceDataObject, prop);

                    foreach (var item in itemsToMap)
                    {
                        destinationObject = Activator.CreateInstance(destinationObject.GetType());
                        destinationObject = ItemMapper.Map(sourceDataObject, destinationObject);
                        try
                        {
                            var listMap = GetListMap(item, destinationObject);

                            listMap(item, destinationObject);

                            list.Add((T)destinationObject);
                        }
                        catch (System.Exception ex)
                        {
                            string Message = string.Format("Problem with list mapper, more: {0}", ex.Message);
                            throw new System.Exception(Message, ex);
                        }
                    }

                    return list;
                }
                catch (System.Exception ex)
                {
                    string Message = string.Format("Problem with mapper, more: {0}", ex.Message);
                    throw new System.Exception(Message, ex);
                }
            }
        }


        private IEnumerable<object> GetItemsToMap(object sourceDataObject, Tuple<string, List<PropertyInfoEx>> propertyPath)
        {
            //if (ItemsToMap == null)
            if (ItemsToMapFunc == null)
            {
                try
                {
                    var inputParameter = Expression.Parameter(typeof(object), "inputParameter");
                    var parameter = Expression.Convert(inputParameter, sourceDataObject.GetType());
                    var property = Expression.Property(parameter, propertyPath.Item2[0].SystemProperty.Name);
                    ItemsToMapFunc = Expression.Lambda<Func<object, IEnumerable<object>>>(property, inputParameter).Compile();
                }
                catch (System.Exception ex)
                {
                    string Message = string.Format("Problem with get item to map, more: {0}", ex.Message);
                    throw new System.Exception(Message, ex);
                }
            }

            ItemsToMap = ItemsToMapFunc(sourceDataObject);

            return ItemsToMap;
        }

        private Action<object, object> GetListMap(object item, object destinationObject)
        {
            if (ListMap == null)
            {
                List<Expression> expression = new List<Expression>();
                var inputValueLine = Expression.Parameter(typeof(object), "item");
                var inputResult = Expression.Parameter(typeof(object));
                foreach (var propertyPath in CollectionProperties)
                {
                    expression.Add(getPropertyMapExpression(item, destinationObject, propertyPath, inputValueLine, inputResult));
                }

                var expressionBlock = Expression.Block(expression);

                ListMap = Expression.Lambda<Action<object, object>>(expressionBlock, inputValueLine, inputResult).Compile();
            }

            return ListMap;
        }
    }
}
