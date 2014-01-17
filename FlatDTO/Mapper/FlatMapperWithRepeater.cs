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
        private List<Tuple<string, List<PropertyInfoEx>>> RegularProperties;
        private List<Tuple<string, List<PropertyInfoEx>>> AllProperties;
        private IDTOMapper<T> ItemMapper = null;

        private IEnumerable<object> ItemsToMap = null;
        //private Action<object, object> ListMap = null;
        private Dictionary<string, Action<object, object>> MapperDictionary = new Dictionary<string,Action<object,object>>();
        private Func<object, IEnumerable<object>> ItemsToMapFunc = null;

        public FlatMapperWithRepeater(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties, IDTOMapper<T> itemMapper)
        {
            this.ItemMapper = itemMapper;
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            CollectionProperties = properties.Where(x => x.Item2.Exists(y => y.IsCollection)).ToList();
            RegularProperties = properties.Where(x => !x.Item2.Exists(y => y.IsCollection)).ToList();
            AllProperties = properties;
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

        protected List<T> MapCollectionProperties(object sourceDataObject, object destinationObject)
        {
            List<T> list = new List<T>();

            if (CollectionProperties.Count > 0)
            {
                try
                {
                    var itemsToMap = GetItemsToMap(sourceDataObject, CollectionProperties.FirstOrDefault());

                    if (itemsToMap != null && itemsToMap.Count() > 0)
                    {
                        foreach (var item in itemsToMap)
                        {
                            destinationObject = Activator.CreateInstance(destinationObject.GetType());
                            try
                            {
                                var effectiveProperties = new List<Tuple<string, List<PropertyInfoEx>>>();

                                foreach (var colProperty in CollectionProperties)
                                {
                                    var propertyInfoExList = new List<PropertyInfoEx>();
                                    bool listRootFound = false;
                                    foreach (var property in colProperty.Item2)
                                    {
                                        if (property.IsCollection && property.HasComplexObjectDescriptor)
                                            propertyInfoExList.Add(property);

                                        if (listRootFound)
                                            propertyInfoExList.Add(property);

                                        if (property.IsCollection)
                                            listRootFound = true;
                                    }

                                    effectiveProperties.Add(new Tuple<string, List<PropertyInfoEx>>(colProperty.Item1, propertyInfoExList));
                                    /*if (colProperty.Item2[0].IsCollection && !colProperty.Item2[0].HasComplexObjectDescriptor)
                                    {
                                        colProperty.Item2.RemoveAt(0);
                                    }*/
                                }

                                destinationObject = performMapping(item, destinationObject, effectiveProperties);

                                list.Add((T)destinationObject);
                            }
                            catch (System.Exception ex)
                            {
                                string Message = string.Format("Problem with list mapper, more: {0}", ex.Message);
                                throw new System.Exception(Message, ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    string Message = string.Format("Problem with mapper, more: {0}", ex.Message);
                    throw new System.Exception(Message, ex);
                }
            }

            return list;
        }

        public override object Map(object sourceDataObject, object destinationObject)
        {

            //Map the collection properties
            var resultList = MapCollectionProperties(sourceDataObject, destinationObject);

            //Map the non collection related fields
            if (resultList.Count > 0)
            {
                foreach (var item in resultList)
                {
                    performMapping(sourceDataObject, item, RegularProperties);
                }

                return resultList;
            }
            else
            {
                destinationObject = performMapping(sourceDataObject, destinationObject,RegularProperties);
                return destinationObject;
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
                    Expression collectionProperty = parameter;
                    foreach (var property in propertyPath.Item2)
                    {
                        //collectionProperty = Expression.Property(parameter, propertyPath.Item2[0].SystemProperty.Name);
                        collectionProperty = Expression.PropertyOrField(collectionProperty, property.SystemProperty.Name);
                        if (property.IsCollection)
                            break;
                    }
                        //var property = Expression.Property(parameter, propertyPath.Item2[0].SystemProperty.Name);
                    ItemsToMapFunc = Expression.Lambda<Func<object, IEnumerable<object>>>(collectionProperty, inputParameter).Compile();
                }
                catch (System.Exception ex)
                {
                    string Message = string.Format("Problem with get collecyion item to map, more: {0}", ex.Message);
                    throw new System.Exception(Message, ex);
                }
            }

            ItemsToMap = ItemsToMapFunc(sourceDataObject);

            return ItemsToMap;
        }

        private object performMapping(object item, object destinationObject, List<Tuple<string, List<PropertyInfoEx>>> propertiesToMap)
        {
            var regularProperties = new List<Tuple<string, List<PropertyInfoEx>>>();
            var complexDestriptorProperties = new List<Tuple<string, List<PropertyInfoEx>>>();

            foreach (var path in propertiesToMap)
            {
                if (path.Item2.Last().HasComplexObjectDescriptor)
                    complexDestriptorProperties.Add(path);
                else
                    regularProperties.Add(path);
            }

            if (regularProperties.Count > 0)
            {
                var RegularMappingFunc = GetListMapFunc(item, destinationObject, regularProperties);

                RegularMappingFunc(item, destinationObject);
            }
            if (complexDestriptorProperties.Count > 0)
            {
                destinationObject = this.MapComplexObject(item, destinationObject, complexDestriptorProperties);
            }
            
            return destinationObject;
        }

        private Action<object, object> GetListMapFunc(object item, object destinationObject, List<Tuple<string, List<PropertyInfoEx>>> propertiesToMap)
        {
            var key = item.GetType().FullName;
            Action<object, object> MapFunc = null;
            
            MapperDictionary.TryGetValue(key, out MapFunc);

            if (MapFunc == null)
            {
                List<Expression> expression = new List<Expression>();
                var inputValueLine = Expression.Parameter(typeof(object), "item");
                var inputResult = Expression.Parameter(typeof(object));
                foreach (var propertyPath in propertiesToMap)
                {
                    expression.Add(getPropertyMapExpression(item, destinationObject, propertyPath, inputValueLine, inputResult));
                }

                var expressionBlock = Expression.Block(expression);

                MapFunc = Expression.Lambda<Action<object, object>>(expressionBlock, inputValueLine, inputResult).Compile();
                MapperDictionary.Add(key, MapFunc);
            }

            return MapFunc;
        }
    }
}
