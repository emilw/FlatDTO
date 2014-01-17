using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace FlatDTO
{
    public abstract class MapperEngine: IMapperEngine
    {
        protected List<IComplexObjectDescriptor> _descriptors;
        private string _typeName;

        public MapperEngine() { }

        public MapperEngine(IEnumerable<IComplexObjectDescriptor> descriptors = null)
        {
            if(descriptors != null)
                _descriptors = descriptors.ToList();
        }

        public BaseClass.DTOMapper<T> Create<T>(Type type, string[] properties, List<PolymorficTypeConverterInstruction> manualPolymorficConverter, string key)
        {
            _typeName = "FlatDTO" + key;

            var propertyInfos = GetPropertiesToUse(type, properties, manualPolymorficConverter);

            var destinationType = CreateDTOType<T>(type, propertyInfos);

            var mapper = CreateMapper<T>(type, destinationType, propertyInfos);

            mapper.MapperEngine = this;

            return mapper;
        }

        protected virtual string TypeName
        {
            get
            {
                return _typeName;
            }
        }


        protected virtual BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            throw new NotImplementedException();
        }

        protected virtual List<Tuple<string, List<PropertyInfoEx>>> GetPropertiesToUse(Type type, string[] propertyPath, List<PolymorficTypeConverterInstruction> manualPolymorficConverter)
        {

            var result = new List<Tuple<string, List<PropertyInfoEx>>>();

            foreach(var path in propertyPath)
            {
                var key = path.Replace('.', '_');

                var properties = path.Split('.');
                
                var activeType = type;
                var propertyInfoList = new List<PropertyInfoEx>();

                var currentPath = "";

                foreach (var property in properties)
                {
                    if (currentPath == "")
                        currentPath = property;
                    else
                        currentPath = currentPath + "." + property;

                    var polymorficType = manualPolymorficConverter.FirstOrDefault(x => string.Equals(x.PropertyPath, currentPath, StringComparison.InvariantCultureIgnoreCase));

                    //Check if the properties exists on the type
                    var propertyInfo = activeType.GetProperty(property);

                    Type listType = null;

                    if (propertyInfo != null && IsCollectionType(propertyInfo.PropertyType))
                    {
                        try
                        {
                            listType = propertyInfo.PropertyType.GetProperty("Item").PropertyType;
                            //throw new System.Exception("Collection type found at: " + propertyInfo.Name + " " + listType.Name);
                        }
                        catch (System.Exception ex)
                        {
                            throw new NotSupportedException("It seems like there are simple list types specified. That is not supported", ex);
                        }
                    }

                    if (propertyInfo == null)
                        throw new Exception.PropertyDoNotExistException(property, activeType, path);

                    PropertyInfoEx propertyEx = new PropertyInfoEx(propertyInfo);

                    if (polymorficType != null)
                        propertyEx.SetToPolymorficProperty(polymorficType.ConvertToType);

                    if (listType != null)
                    {
                        propertyEx.SetToCollectionProperty(listType);

                        //if (HasComplexObjectDescriptor(listType))
                        //    propertyEx.SetToComplexObjectDescriptorProperty(_descriptors.Where(x => x.HandlesType(listType)).FirstOrDefault());

                    }
                    
                   // if (HasComplexObjectDescriptor(propertyInfo.PropertyType))
                   //     propertyEx.SetToComplexObjectDescriptorProperty(_descriptors.Where(x => x.HandlesType(propertyInfo.PropertyType)).FirstOrDefault());


                    propertyInfoList.Add(propertyEx);

                    activeType = propertyEx.Type;
                }

                var leafProperty = propertyInfoList.Last();

                if (HasComplexObjectDescriptor(leafProperty.PropertyType) || HasComplexObjectDescriptor(leafProperty.ItemType))
                {
                    Type typeToCheck = null;
                    typeToCheck = leafProperty.PropertyType;

                    if (leafProperty.ItemType != null)
                        typeToCheck = leafProperty.ItemType;

                    leafProperty.SetToComplexObjectDescriptorProperty(_descriptors.Where(x => x.HandlesType(typeToCheck)).FirstOrDefault());
                }

                if (!IsSimpleType(leafProperty.SystemProperty.PropertyType) && !leafProperty.HasComplexObjectDescriptor)
                    throw new Exception.PropertyIsNotSimpleTypeException(leafProperty.SystemProperty.Name, leafProperty.Type, activeType, path);

                result.Add(new Tuple<string, List<PropertyInfoEx>>(key, propertyInfoList));
            }

            return result;
        }

        public static bool IsCollectionType(Type type)
        {
            Type collectionType = typeof(ICollection<>);

            if (type.IsGenericType && collectionType.IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == collectionType))
            {
                return true;
            }
            return false;
        }
        public bool HasComplexObjectDescriptor(Type type)
        {
            if (_descriptors == null)
                return false;
            else
                return _descriptors.Exists(x => x.HandlesType(type));
        }

        protected static bool IsSimpleType(Type type)
        {
            if (type == typeof(string) ||
                type == typeof(int) ||
                type == typeof(int?) ||
                type == typeof(long) ||
                type == typeof(long?) ||
                type == typeof(DateTime) ||
                 type == typeof(DateTime?) ||
                type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(bool) ||
                type == typeof(bool?) ||
                type == typeof(Guid) ||
                type == typeof(Guid?))
                return true;
            else
                return false;
        }


        protected virtual Type CreateDTOType<T>(Type type, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveAssembly(string folderPath)
        {
            throw new NotImplementedException();
        }
    }
}
