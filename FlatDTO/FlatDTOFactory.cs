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

        public Dictionary<string, BaseClass.DTOMapper> CurrentListOfMappers
        {
            get
            {
                return MapperList;
            }
        }

        public Type[] CurrentListOfDTOTypes
        {
            get
            {
                return MapperList.Values.Select(x => x.DestinationType).ToArray();
            }
        }

        private BaseClass.DTOMapper GetDTOMapper<T>(object dataObject, string[] properties)
        {

            if (dataObject == null)
                throw new ArgumentNullException("dataObject");
            if (properties == null)
                throw new ArgumentNullException("properties");
            if (properties.Count() == 0)
                throw new System.Exception("The list of properties to transform was empty");

            var type = dataObject.GetType();

            string key = type.FullName;
            foreach (var property in properties)
                key = key + property;

            BaseClass.DTOMapper mapper = null;

            if (!MapperList.ContainsKey(key))
            {
                var mapperEngine = new MapperEngine();
                mapper = mapperEngine.CreateMapper<T>(type, properties);
                mapper.Key = key;
                mapper.CreatedDateTime = DateTime.Now;
                MapperList.Add(key, mapper);
                
                if(Compiling != null)
                    Compiling(key, new EventArgs());
            }
            else
            {
                mapper = MapperList[key];
                if(CacheHit != null)
                    CacheHit(key, new EventArgs());
            }

            return mapper;
        }

        public T[] Create<T>(object[] dataObject, string[] properties)
        {
            
            if (dataObject.Count() == 0)
                return new List<T>().ToArray();

            var mapper = GetDTOMapper<T>(dataObject[0], properties);
            
            return mapper.Map<T>(dataObject);
        }

        public T Create<T>(object dataObject, string[] properties)
        {
            var mapper = GetDTOMapper<T>(dataObject, properties);

            return mapper.Map<T>(dataObject);
        }

        public event EventHandler Compiling;
        public event EventHandler CacheHit;
    }
}
