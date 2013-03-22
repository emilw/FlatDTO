using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public interface IDTOMapperInfo
    {
        Type SourceType { get; set; }
        Type DestinationType { get; set; }

        string Key { get; set; }
        DateTime CreatedDateTime { get; set; }
        
    }
}
