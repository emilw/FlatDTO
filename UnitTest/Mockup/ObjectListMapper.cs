using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.Mockup
{
    public class ObjectListMapper<T> : FlatDTO.BaseClass.DTOMapper<T>
    {
        public override object Map(object sourceDataObject, object destinationObject)
        {
            var input = (Data.FlatDataType)sourceDataObject;

            var result = new List<Data.FlatDataType>();
            for (int i = 0; i < 11; i++)
            {
                result.Add(new Data.FlatDataType() { DecimalValue = i });
            }
            return result;
        }
    }
}
