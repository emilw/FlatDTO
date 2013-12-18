using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Exception
{
    public class PropertyIsNotSimpleTypeException : ApplicationException
    {
        string _message;

        public PropertyIsNotSimpleTypeException(string propertyName, Type propertyType, Type type, string fullPath)
        {
             _message = string.Format("The property {0} with type {1} is not a primitive type on parent type {2} or it have no appropriate descriptor defined. Full property path used {3}", propertyName, propertyType, type, fullPath);
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
