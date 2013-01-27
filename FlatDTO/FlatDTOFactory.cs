using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public class FlatDTOFactory
    {
        public FlatDTOFactory()
        {
            MapperList = new Dictionary<string, BaseClass.DTOMapper>();
        }
        private Dictionary<string, BaseClass.DTOMapper> MapperList { get; set; }

        private BaseClass.DTOMapper GetDTOMapper(object[] dataObject, string[] properties)
        {
            var type = dataObject[0].GetType();
            string key = type.FullName;
            foreach (var property in properties)
                key = key + property;

            BaseClass.DTOMapper mapper = null;

            if (!MapperList.ContainsKey(key))
            {
                mapper = MapperEngine.CreateMapper(type, properties);
                mapper.Key = key;
                mapper.CreatedDateTime = DateTime.Now;
                MapperList.Add(key, mapper);
            }
            else
            {
                mapper = MapperList[key];
            }

            return mapper;
        }

        public FlatDTO.BaseClass.DTO[] Create(object[] dataObject, string[] properties)
        {
            var mapper = GetDTOMapper(dataObject, properties);

            return mapper.Map(dataObject);
        }
    }
}
