using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public interface IDTOMapper<TDTOBase> : IDTOMapperInfo
    {
        TDTOBase[] Map(object[] dataObject);

        TDTOBase Map(object dataObject);

        object Map(object sourceDataObject, object destinationObject);

        TOriginal[] UnMap<TOriginal>(object[] dataObject);

        TOriginal UnMap<TOriginal>(object dataObject);

        object UnMap(object sourceDataObject, object destinationObject);
    }
}
