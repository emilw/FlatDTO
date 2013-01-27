using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
    public class CreateMethodReflectionEmit : BaseTest
    {
        public override object Map(object typeA, object typeB)
        {
            var typeAA = (TypeA)typeA;
            var typeBB = (TypeB)typeB;

            typeAA.StringA = typeBB.StringB;

            return typeAA;
        }
    }

    public abstract class BaseTest
    {
        public abstract object Map(object typeA, object typeB);
    }


    public class TypeA
    {
        public string StringA {get; set;}
    }

    public class TypeB
    {
        public string StringB { get; set; }
    }


}
