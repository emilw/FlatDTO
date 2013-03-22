using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public interface IMapperEngine
    {
        /// <summary>
        /// Creates the mapper.
        /// This function is the only one that is puplic.
        /// Derrive from MapperEngine and ovverride the CreateMapper and CreateDTO function to generate your own types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="properties"></param>
        /// <param name="manualPolymorficConverter"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        BaseClass.DTOMapper<T> Create<T>(Type type, string[] properties, List<PolymorficTypeConverterInstruction> manualPolymorficConverter, string key);
    }
}
