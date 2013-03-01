using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FlatDTO
{
    public class FlatDTOFactory
    {
        MapperEngine _mapperEngine;
        public FlatDTOFactory()
        {
            MapperList = new Dictionary<string, BaseClass.DTOMapper>();
            _mapperEngine = new MapperEngine();
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

        private BaseClass.DTOMapper GetDTOMapper<T>(object dataObject, string[] properties, List<PolymorficTypeConverterInstruction> manualPolymorficConverter = null)
        {

            if (dataObject == null)
                throw new ArgumentNullException("dataObject");
            if (properties == null)
                throw new ArgumentNullException("properties");
            if (properties.Count() == 0)
                throw new System.Exception("The list of properties to transform was empty");

            var type = dataObject.GetType();

            var key = GetKey<T>(type, properties, manualPolymorficConverter);

            BaseClass.DTOMapper mapper = null;

            if (!MapperList.ContainsKey(key))
            {
                var mapperEngine = new MapperEngine();
                if (manualPolymorficConverter == null)
                    manualPolymorficConverter = new List<PolymorficTypeConverterInstruction>();
                mapper = _mapperEngine.CreateMapper<T>(type, properties, manualPolymorficConverter, key);
                mapper.Key = key;
                mapper.CreatedDateTime = DateTime.Now;
                MapperList.Add(key, mapper);
                
                if(Compiling != null)
                    Compiling(mapper, new EventArgs());
            }
            else
            {
                mapper = MapperList[key];
                if(CacheHit != null)
                    CacheHit(mapper, new EventArgs());
            }

            return mapper;
        }

        public T[] Create<T>(object[] dataObject, string[] properties, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            
            if (dataObject.Count() == 0)
                return new List<T>().ToArray();

            var mapper = GetDTOMapper<T>(dataObject[0], properties, polymorficTypeConverter);
            
            return mapper.Map<T>(dataObject);
        }

        public T Create<T>(object dataObject, string[] properties, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            var mapper = GetDTOMapper<T>(dataObject, properties, polymorficTypeConverter);

            return mapper.Map<T>(dataObject);
        }

        private string GetKey<T>(Type inputType, string[] properties, List<PolymorficTypeConverterInstruction> manualPolymorficConverter = null)
        {
            List<byte> buffer = new List<byte>();

            buffer.AddRange(Encoding.UTF8.GetBytes(inputType.FullName));
            buffer.AddRange(Encoding.UTF8.GetBytes(typeof(T).FullName));

            foreach (string property in properties)
            {
                buffer.AddRange(Encoding.UTF8.GetBytes(property));
            }
            if (manualPolymorficConverter != null)
            {
                foreach (var converter in manualPolymorficConverter)
                {
                    buffer.AddRange(Encoding.UTF8.GetBytes(converter.PropertyPath));
                    buffer.AddRange(Encoding.UTF8.GetBytes(converter.ConvertToType.FullName));
                }
            }

            byte[] hash = new SHA256Managed().ComputeHash(buffer.ToArray());

            return BitConverter.ToString(hash)
                .Replace("-", String.Empty);
        }

        /// <summary>
        /// This event handler returns if the mapper needed to be compiled or not
        /// The sender includes the mapper that was compiled
        /// </summary>
        public event EventHandler Compiling;
        /// <summary>
        /// This event handler returns if the mapper was fetched from cache
        /// The sender includes the mapper that was compiled
        /// </summary>
        public event EventHandler CacheHit;
    }
}
