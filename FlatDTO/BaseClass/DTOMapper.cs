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
                var dto = Map<T>(data);
                result.Add(dto);
            }

            return result.ToArray();
        }

        public T Map<T>(object dataObject)
        {
            var dto = Activator.CreateInstance(DestinationType);
            return (T)Map(dataObject, dto);
        }

        public abstract object Map(object sourceDataObject, object destinationObject);
    }
}
