using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Exception
{
    public class PropertyDoNotExistException : ApplicationException
    {
        string _message;
        
        public PropertyDoNotExistException(string propertyName, Type type, string fullPath)
        {
             _message = string.Format("The property {0} does not exists on type {1}. Full property path used {2}", propertyName, type, fullPath);
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
