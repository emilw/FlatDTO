using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public interface IComplexObjectDescriptor
    {
        bool HandlesType(Type type);
        string Describe(object @object);
    }
}
