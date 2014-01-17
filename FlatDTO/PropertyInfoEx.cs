using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FlatDTO
{
    public class PropertyInfoEx
    {
        public Type PolymorficType = null;
        
        private PropertyInfo _systemProperty;

        public Type ItemType = null;

        public IComplexObjectDescriptor ComplexObjectDescriptor { get; set; }

        public void SetToCollectionProperty(Type itemType)
        {
            this.ItemType = itemType;
        }

        public void SetToPolymorficProperty(Type polymorficType)
        {
            this.PolymorficType = polymorficType;
        }

        public void SetToComplexObjectDescriptorProperty(IComplexObjectDescriptor complexObjectDescriptor)
        {
            this.ComplexObjectDescriptor = complexObjectDescriptor;
        }

        public PropertyInfoEx(PropertyInfo propertyInfo)
        {
            _systemProperty = propertyInfo;
        }

        public bool HasComplexObjectDescriptor
        {
            get
            {
                if (ComplexObjectDescriptor == null)
                    return false;
                else
                    return true;
            }
        }

        public bool IsPolyMorfic
        {
            get
            {
                if (PolymorficType != null)
                    return true;
                else
                    return false;
            }
        }

        public bool IsCollection
        {
            get
            {
                if (ItemType != null)
                    return true;
                return false;
            }
        }

        public Type Type
        {
            get
            {
                if (IsPolyMorfic)
                    return PolymorficType;
                else if (IsCollection)
                    return ItemType;
                else
                    return SystemProperty.PropertyType;
            }
        }

        public Type PropertyType
        {
            get
            {
                return SystemProperty.PropertyType;
                //return GetNullableType(SystemProperty.PropertyType);
            }
        }

        public static bool IsNullable(Type type)
        {
            return nullableTypes.Contains(type);
        }

        private static Type[] nullableTypes
        {
            get
            {
                return new Type[]
                {
                    typeof(int?),
                    typeof(DateTime?),
                    typeof(decimal?),
                    typeof(bool?),
                    typeof(Guid?)
                };
            }
        }

        private static Type GetNullableType(Type type)
        {
            if(type == typeof(int)){
                return typeof(int?);
            }
            else if(type == typeof(long)){
                return typeof(long?);
            }
            else if(type == typeof(DateTime)){
                return typeof(DateTime?);
            }
            else if(type == typeof(decimal)){
                return typeof(decimal?);
            }
            else if(type == typeof(bool)){
                return typeof(bool?);
            }
            else if (type == typeof(Guid)){
                return typeof(Guid?);
            }
            else{
                return type;
            }

        }

        public PropertyInfo SystemProperty
        {
            get
            {
                return _systemProperty;
            }
        }
    }
}
