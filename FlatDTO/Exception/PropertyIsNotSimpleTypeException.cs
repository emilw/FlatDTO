using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Exception
{
    public class PropertyIsNotSimpleTypeException : ApplicationException
    {
        string _message;

        public PropertyIsNotSimpleTypeException(string propertyName, Type type, string fullPath)
        {
             _message = string.Format("The property {0} is not a primitive type on parent type {1}. Full property path used {2}", propertyName, type, fullPath);
        }

        public override string Message
        {
            get
            {
                return Message;
            }
        }
    }
}
