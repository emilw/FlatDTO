using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Exception
{
    public class MapperException<T> : ApplicationException
    {
        string _message;

        public MapperException(BaseClass.DTOMapper<T> mapper, System.Exception innerException, bool isMapperException = true)
        {
            
            string mapperText = "";
            if(isMapperException)
                mapperText = "Map";
            else
                mapperText = "UnMap";

             _message = string.Format("An exeption occured in the mapper with key {0}, it seems to be an issue in the {1} method. Please revise the generation of the {1} code in the MapperEngine or check the input data so it applies to the pattern used by the {1} method.", mapper.Key, mapperText);
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
