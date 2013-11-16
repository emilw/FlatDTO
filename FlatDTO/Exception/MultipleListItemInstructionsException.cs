using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Exception
{
    public class MultipleListItemInstructionsException : ApplicationException
    {
        string _message;

        public MultipleListItemInstructionsException(string[] propertyName)
        {
            _message = "Instructions to map different lists exists, FlatDTO only handles one mapped list per mapping. These instructions collide: ";

            foreach (var property in propertyName)
            {
                _message = _message + " " + property;
            }

             
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
