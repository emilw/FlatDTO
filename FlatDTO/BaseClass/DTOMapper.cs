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
        public DTO[] Map(object[] dataObject)
        {
            var result = new List<DTO>();

            foreach (var data in dataObject)
            {
                var dto = (DTO)Activator.CreateInstance(DestinationType);
                dto = Map(data, dto);
                result.Add(dto);
            }

            return result.ToArray();
        }

        public abstract DTO Map(object sourceDataObject, DTO destinationObject);
    }
}
