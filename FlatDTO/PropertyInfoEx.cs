using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FlatDTO
{
    public class PropertyInfoEx
    {
        private Type _polyMorficType = null;
        
        private PropertyInfo _systemProperty;

        public PropertyInfoEx(PropertyInfo propertyInfo, Type polymorficType = null)
        {
            _systemProperty = propertyInfo;
            _polyMorficType = polymorficType;
        }

        public bool IsPolyMorfic
        {
            get
            {
                if (_polyMorficType != null)
                    return true;
                else
                    return false;
            }
        }

        public Type Type
        {
            get
            {
                if (IsPolyMorfic)
                    return _polyMorficType;
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
