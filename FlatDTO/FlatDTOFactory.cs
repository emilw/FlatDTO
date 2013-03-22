using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using FlatDTO.BaseClass;

namespace FlatDTO
{
    public class FlatDTOFactory
    {
        public FlatDTOFactory()
        {
            MapperList = new Dictionary<string, IDTOMapperInfo>();
        }

        private Dictionary<string, IDTOMapperInfo> MapperList { get; set; }

        public IDTOMapperInfo[] RegisteredMappers
        {
            get
            {
                return MapperList.Values.ToArray();
            }
        }

        public DTOMapper<TDTOBase> Create<TDTOBase>(Type sourceType, string[] properties, IMapperEngine mapperEngine, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            var mapper = GetDTOMapper<TDTOBase>(sourceType, properties, mapperEngine, polymorficTypeConverter);
            return mapper;
        }

        public TDTOBase[] Map<TDTOBase>(Type sourceType, string[] properties, IMapperEngine mapperEngine, object[] data, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            return Create<TDTOBase>(sourceType, properties, mapperEngine, polymorficTypeConverter).Map(data);
        }

        public TDTOBase Map<TDTOBase>(Type sourceType, string[] properties, IMapperEngine mapperEngine, object data, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            return Create<TDTOBase>(sourceType, properties, mapperEngine, polymorficTypeConverter).Map(data);
        }

        public TOriginal[] UnMap<TDTOBase, TOriginal>(Type sourceType, string[] properties, IMapperEngine mapperEngine, object[] data, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            return Create<TDTOBase>(sourceType, properties, mapperEngine, polymorficTypeConverter).UnMap<TOriginal>(data);
        }

        public TOriginal UnMap<TDTOBase, TOriginal>(Type sourceType, string[] properties, IMapperEngine mapperEngine, object data, List<PolymorficTypeConverterInstruction> polymorficTypeConverter = null)
        {
            return Create<TDTOBase>(sourceType, properties, mapperEngine, polymorficTypeConverter).UnMap<TOriginal>(data);
        }

        private BaseClass.DTOMapper<TDTOBase> GetDTOMapper<TDTOBase>(Type sourceType, string[] properties, IMapperEngine mapperEngine, List<PolymorficTypeConverterInstruction> manualPolymorficConverter = null)
        {

            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (properties == null)
                throw new ArgumentNullException("properties");
            if (properties.Count() == 0)
                throw new System.Exception("The list of properties to transform was empty");

            var key = GetKey<TDTOBase>(sourceType, properties, mapperEngine.GetType(), manualPolymorficConverter);

            BaseClass.DTOMapper<TDTOBase> mapper = null;

            if (!MapperList.ContainsKey(key))
            {
                if (manualPolymorficConverter == null)
                    manualPolymorficConverter = new List<PolymorficTypeConverterInstruction>();
                mapper = mapperEngine.Create<TDTOBase>(sourceType, properties, manualPolymorficConverter, key);
                mapper.Key = key;
                mapper.CreatedDateTime = DateTime.Now;
                MapperList.Add(key, mapper);
                
                if(Compiling != null)
                    Compiling(mapper, new EventArgs());
            }
            else
            {
                mapper = (BaseClass.DTOMapper<TDTOBase>)MapperList[key];
                if(CacheHit != null)
                    CacheHit(mapper, new EventArgs());
            }

            return mapper;
        }


        /// <summary>
        /// Generates a unique hash based on the input parameters.
        /// </summary>
        /// <typeparam name="TDTOBase"></typeparam>
        /// <param name="inputType"></param>
        /// <param name="properties"></param>
        /// <param name="mapperEngineType"></param>
        /// <param name="manualPolymorficConverter"></param>
        /// <returns></returns>
        protected virtual string GetKey<TDTOBase>(Type inputType, string[] properties, Type mapperEngineType, List<PolymorficTypeConverterInstruction> manualPolymorficConverter = null)
        {
            List<byte> buffer = new List<byte>();

            buffer.AddRange(Encoding.UTF8.GetBytes(inputType.FullName));
            buffer.AddRange(Encoding.UTF8.GetBytes(typeof(TDTOBase).FullName));
            buffer.AddRange(Encoding.UTF8.GetBytes(mapperEngineType.FullName));

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
