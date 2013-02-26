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
            typeAA.StringAA = ((TypeAAB)typeBB.ComplexString).StringAAB;

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
        public string StringAA { get; set; }
    }

    public class TypeAA
    {
        public string StringAA { get; set; }
    }

    public class TypeAAB: TypeAA
    {
        public string StringAAB { get; set; }
    }

    public class TypeB
    {
        public string StringB { get; set; }
        public TypeAA ComplexString { get; set; }
    }


}
