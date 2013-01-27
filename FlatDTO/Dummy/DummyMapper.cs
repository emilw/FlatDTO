using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO.Dummy
{
    public class DummyMapper : BaseClass.DTOMapper
    {

        public override BaseClass.DTO Map(object dataObject, FlatDTO.BaseClass.DTO dto)
        {
            /*var dto = new DummyDTO();
            dataObject*/

            return new BaseClass.DTO();
        }
    }

    public class DummyDTO : BaseClass.DTO
    {

    }
}
