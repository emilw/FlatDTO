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

        public static PropertyInfoEx CreateCollectionProperty(PropertyInfo propertyInfo, Type itemType)
        {
            var result = new PropertyInfoEx(propertyInfo);
            result.ItemType = itemType;
            return result;
        }

        public static PropertyInfoEx CreatePolymorficProperty(PropertyInfo propertyInfo, Type polymorficType)
        {
            var result = new PropertyInfoEx(propertyInfo);
            result.PolymorficType = polymorficType;
            return result;
        }


        public PropertyInfoEx(PropertyInfo propertyInfo)
        {
            _systemProperty = propertyInfo;
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
        public PropertyInfo SystemProperty
        {
            get
            {
                return _systemProperty;
            }
        }
    }
}
