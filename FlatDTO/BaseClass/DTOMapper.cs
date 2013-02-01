using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.BaseClass
{
    public abstract class DTOMapper
    {
        public Type SourceType {get; set;}
        public Type DestinationType { get; set; }

        public string Key { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public T[] Map<T>(object[] dataObject)
        {
            var result = new List<T>();

            foreach (var data in dataObject)
            {
                var dto = Activator.CreateInstance(DestinationType);
                dto = Map(data, dto);
                result.Add((T)dto);
            }

            return result.ToArray();
        }

        public abstract object Map(object sourceDataObject, object destinationObject);
    }
}
