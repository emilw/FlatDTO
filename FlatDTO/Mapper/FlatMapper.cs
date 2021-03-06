﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FlatDTO.Mapper
{
    public class FlatMapper<T> : FlatDTO.BaseClass.DTOMapper<T>
    {
        List<Tuple<string, List<PropertyInfoEx>>> ComplexObjectDescriptor = new List<Tuple<string, List<PropertyInfoEx>>>();
//        Action<object, object> MapComplexObjectAction;

        Dictionary<string, Action<object, object>> MapComplexObjectDictionary = new Dictionary<string,Action<object,object>>();

        public FlatMapper()
        {
            
        }

        public void Init(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            SourceType = sourceType;
            DestinationType = destinationType;

            //Separate all complexdescriptor properties from the rest
            properties.ForEach(x =>
            {
                if (x.Item2.Last().HasComplexObjectDescriptor && !x.Item2.Exists(y => y.IsCollection))
                    ComplexObjectDescriptor.Add(x);
            });
        }

        public override object Map(object sourceDataObject, object destinationObject)
        {
            //Legacy IL code mapping
            var ilMappedObjects = ILMap(sourceDataObject, destinationObject);
            var expressionMappedObjects = ExpMap(sourceDataObject, ilMappedObjects);
            return expressionMappedObjects;
        }

        public virtual object ILMap(object sourceDataObject, object destinationObject)
        {
            throw new NotImplementedException("ILMap is not implemented");
        }

        protected object ExpMap(object soruceDataObject, object destinationObject)
        {
            if (ComplexObjectDescriptor.Count() > 0)
                return MapComplexObject(soruceDataObject, destinationObject, this.ComplexObjectDescriptor);
            else
                return destinationObject;
        }

        protected object MapComplexObject(object sourceDataObject, object destinationObject, List<Tuple<string, List<PropertyInfoEx>>> propertiesToMap)
        {
            var key = sourceDataObject.GetType().FullName;
            Action<object, object> MapComplexObjectAction = null;

            MapComplexObjectDictionary.TryGetValue(key, out MapComplexObjectAction);

            if (MapComplexObjectAction == null)
            {
                List<Expression> expression = new List<Expression>();
                var inputValueLine = Expression.Parameter(typeof(object));
                var inputResult = Expression.Parameter(typeof(object));
                foreach (var propertyPath in propertiesToMap.Where(x => x.Item2.Exists(y => y.HasComplexObjectDescriptor)))
                {
                    expression.Add(getPropertyMapExpression(sourceDataObject, destinationObject, propertyPath, inputValueLine, inputResult));
                }

                var expressionBlock = Expression.Block(expression);

                MapComplexObjectAction = Expression.Lambda<Action<object, object>>(expressionBlock, inputValueLine, inputResult).Compile();
                MapComplexObjectDictionary.Add(key, MapComplexObjectAction);
            }

            MapComplexObjectAction(sourceDataObject,destinationObject);

            return destinationObject;
        }


        protected Expression getPropertyMapExpression(object item, object destinationObject, Tuple<string, List<PropertyInfoEx>> propertyPath,
                                                    ParameterExpression input, ParameterExpression output)
        {
            var valueLine = Expression.Convert(input, item.GetType());

            Expression valueProperty = valueLine;
            IComplexObjectDescriptor descriptor = null;

            //Get the property from the source, iterate to the end of the list, it's ordered
            foreach (var property in propertyPath.Item2)
            {

                if (property.IsCollection && property.HasComplexObjectDescriptor)
                {
                    valueProperty = Expression.Constant(item);
                }
                else
                {
                    valueProperty = Expression.PropertyOrField(valueProperty, property.SystemProperty.Name);
                }

                if (property.IsPolyMorfic)
                {
                    valueProperty = Expression.Convert(valueProperty, property.PolymorficType);
                }
                
                descriptor = property.ComplexObjectDescriptor;
            }

            if (descriptor != null)
            {

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

                if (PropertyInfoEx.IsNullable(resultProperty.Type))
                {
                    valueProperty = Expression.Convert(valueProperty, resultProperty.Type);
                    //valueProperty = Expression.PropertyOrField(valueProperty, "Value");
                }
                //Assign the property from the source property to the result property
                var assignExpression = Expression.Assign(resultProperty, valueProperty);

                return assignExpression;
            }
        }

        /*
        private Expression getPropertyMapExpression(object item, object destinationObject, Tuple<string, List<PropertyInfoEx>> propertyPath,
                                                            ParameterExpression input, ParameterExpression output)
        {
            var valueLine = Expression.Convert(input, item.GetType());

            Expression valueProperty = valueLine;
            IComplexObjectDescriptor descriptor = null;

            //Get the property from the source, iterate to the end of the list, it's ordered
            foreach (var property in propertyPath.Item2)
            {
                valueProperty = Expression.PropertyOrField(valueProperty, property.SystemProperty.Name);

                if(property.HasComplexObjectDescriptor)
                {
                    descriptor = property.ComplexObjectDescriptor;
                }
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

            return null;
        }*/
    }
}
