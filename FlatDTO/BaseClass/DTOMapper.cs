using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.BaseClass
{
    public abstract class DTOMapper<TDTOBase> : IDTOMapper<TDTOBase>
    {
        public Type SourceType {get; set;}
        public Type DestinationType { get; set; }

        public string Key { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public TDTOBase[] Map(object[] dataObject)
        {
            var result = new List<TDTOBase>();

            foreach (var data in dataObject)
            {
                var dto = ObjectMap(data);
                if (FlatDTO.MapperEngine.IsCollectionType(dto.GetType()))
                {
                    result.AddRange((List<TDTOBase>)dto);
                }
                else
                {
                    result.Add((TDTOBase)dto);
                }
            }

            return result.ToArray();
        }

        public TDTOBase Map(object dataObject)
        {
            //return (TDTOBase)ObjectMap(dataObject);

            var objects = new object[]{dataObject};

            return Map(objects).FirstOrDefault();
        }

        private object ObjectMap(object dataObject)
        {
            try
            {
                var dto = Activator.CreateInstance(DestinationType);
                return Map(dataObject, dto);
            }
            catch (System.Exception ex)
            {
                throw new Exception.MapperException<TDTOBase>(this, ex, false);
            }

            return default(TDTOBase);
        }



        

        public TOriginal[] UnMap<TOriginal>(object[] dataObject)
        {
            var result = new List<TOriginal>();

            foreach (var data in dataObject)
            {
                var dto = UnMap<TOriginal>(data);
                result.Add(dto);
            }

            return result.ToArray();
        }

        public TOriginal UnMap<TOriginal>(object dataObject)
        {
            try
            {
                var dto = Activator.CreateInstance(DestinationType);
                return (TOriginal)UnMap(dataObject, dto);
            }
            catch (System.Exception ex)
            {
                throw new Exception.MapperException<TDTOBase>(this, ex, false);
            }
            
            return default(TOriginal);
        }

        public virtual object Map(object sourceDataObject, object destinationObject)
        {
            throw new NotImplementedException("To be able to use the Map method, implement it in the mapper engine");
        }
        public virtual object UnMap(object sourceDataObject, object destinationObject)
        {
            throw new NotImplementedException("To be able to use the UnMap method, implement it in the mapper engine");
        }

        public bool Repeat { get; set; }


        public IMapperEngine MapperEngine { get; set; }
    }
}
