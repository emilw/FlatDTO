using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public interface IComplexObjectDescriptor
    {
        Type RealTypeOfObject { get; }
        string Describe(object @object);
    }
}
