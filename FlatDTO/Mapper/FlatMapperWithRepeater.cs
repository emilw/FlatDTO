using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.Mapper
{
    public class FlatMapperWithRepeater<T> : Mapper.FlatMapper<T>
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

                    if (itemsToMap == null || itemsToMap.Count() == 0)
                    {
                        var dest = ItemMapper.Map(sourceDataObject, destinationObject);

                        list.Add((T)dest);
                    }
                    else
                    {
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
