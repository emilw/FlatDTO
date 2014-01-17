using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatDTO;

namespace UnitTest.Mockup
{
    public static class Data
    {
        public static FlatDataType[] GetSimpleOneLevelMockupData()
        {
            var data = new List<FlatDataType>();
            data.Add(new FlatDataType() { StringValue = "prop1", DecimalValue = 1, IntegerValue = 2 });
            data.Add(new FlatDataType() { StringValue = "prop2", DecimalValue = 100, IntegerValue = 3 });

            return data.ToArray();
        }

        public static string[] GetSimpleOneLevelMockupDataPropertyStrings()
        {
            var stringPropertyName = "StringValue";
            var decimalPropertyName = "DecimalValue";
            var integerPropertyName = "IntegerValue";

            return new string[] { stringPropertyName, decimalPropertyName, integerPropertyName };
        }

        public class FlatDataType
        {
            public string StringValue { get; set; }
            public decimal DecimalValue { get; set; }
            public int IntegerValue { get; set; }
        }

        public class DTOBase
        {
            public string StringValue { get; set; }
        }

        public class DTOBaseEmpty
        {
        }

        //Advance data
        public static ComplexDataType[] GetComplexTwoLevelMockupData()
        {
            var data = new List<ComplexDataType>();
            var comp1 = new ComplexPropertyLevel2() { StringValue = "ComlpexLevel2" };
            var comp2 = new ComplexPropertyLevel1() { ComplexProperty = comp1, StringValue = "ComplexLevel1" };

            //Line items
            var complexLineItem1 = new ComplexPropertyLevel2() { StringValue = "LineComplexValue1" };
            var complexLineItem2 = new ComplexPropertyLevel2() { StringValue = "LineComplexValue2" };
            var comp1List = new List<ComplexPropertyLevel1>();
            comp1List.Add(new ComplexPropertyLevel1() { StringValue = "Line1", ComplexProperty = complexLineItem1 });
            comp1List.Add(new ComplexPropertyLevel1() { StringValue = "Line2", ComplexProperty = complexLineItem2 });

            //Result
            data.Add(new ComplexDataType() { StringValue = "prop1", DecimalValue = 1, IntegerValue = 2, ComplexProperty = comp2, ComplexPropertyList = comp1List, ComplexPropertyList2 = comp1List});
            data.Add(new ComplexDataType() { StringValue = "prop2", DecimalValue = 100, IntegerValue = 3, ComplexProperty = comp2, ComplexPropertyList = comp1List, ComplexPropertyList2 = comp1List });

            return data.ToArray();
        }

        public static string[] GetComplexTwoLevelMockupDataPropertyStrings()
        {
            var stringPropertyName = "StringValue";
            var decimalPropertyName = "DecimalValue";
            var integerPropertyName = "IntegerValue";
            var stringProperetyNameComplex1 = "ComplexProperty.StringValue";
            var stringProperetyNameComplex2 = "ComplexProperty.ComplexProperty.StringValue";

            return new string[] { stringPropertyName, decimalPropertyName, integerPropertyName, stringProperetyNameComplex1, stringProperetyNameComplex2 };
        }

        public static string[] GetComplexTwoLevelListMockupDataPropertyStrings()
        {
            var result = GetComplexTwoLevelMockupDataPropertyStrings().ToList();
            result.Add("ComplexPropertyList.StringValue");
            result.Add("ComplexPropertyList.ComplexProperty.StringValue");
            return result.ToArray();
        }


        //Advance data
        public static ComplexDataType[] GetComplexTwoLevelMockupDataPolymorfic()
        {
            var data = new List<ComplexDataType>();
            var comp1 = new ComplexPropertyLevel2() { StringValue = "ComlpexLevel2" };
            var comp2 = new ComplexPropertyLevel12() { ComplexProperty = comp1, StringValue = "ComplexLevel1", PolymorficString = "PolymorficString"};
            data.Add(new ComplexDataType() { StringValue = "prop1", DecimalValue = 1, IntegerValue = 2, ComplexProperty = comp2 });
            data.Add(new ComplexDataType() { StringValue = "prop2", DecimalValue = 100, IntegerValue = 3, ComplexProperty = comp2 });

            return data.ToArray();
        }

        public static string[] GetComplexTwoLevelMockupDataPropertyStringsPolymorfic()
        {
            var stringPropertyName = "StringValue";
            var decimalPropertyName = "DecimalValue";
            var integerPropertyName = "IntegerValue";
            var stringProperetyNameComplex1 = "ComplexProperty.StringValue";
            var stringProperetyNameComplex12 = "ComplexProperty.PolymorficString";
            var stringProperetyNameComplex2 = "ComplexProperty.ComplexProperty.StringValue";

            return new string[] { stringPropertyName, decimalPropertyName, integerPropertyName, stringProperetyNameComplex1, stringProperetyNameComplex12, stringProperetyNameComplex2 };
        }

        public class ComplexDataType : FlatDataType
        {
            public ComplexPropertyLevel1 ComplexProperty { get; set; }
            public IList<ComplexPropertyLevel1> ComplexPropertyList { get; set; }
            public IList<ComplexPropertyLevel1> ComplexPropertyList2 { get; set; }
        }

        public class ComplexPropertyLevel1
        {
            public ComplexPropertyLevel2 ComplexProperty { get; set; }
            public string StringValue { get; set; }
        }

        public class ComplexPropertyLevel12 : ComplexPropertyLevel1
        {
            public string PolymorficString { get; set; }
        }

        public class ComplexPropertyLevel2
        {
            public string StringValue { get; set; }
        }

        public class ComplexPropertyDescriptor : IComplexObjectDescriptor
        {
            private Type _realType;

            public ComplexPropertyDescriptor(Type connectToType)
            {
                _realType = connectToType;
            }

            public bool HandlesType(Type type)
            {
                return _realType == type;
            }

            protected void checkAndThrow(object @object)
            {
                if (!HandlesType(@object.GetType()))
                    throw new Exception(string.Format("The object type: {0} can not be handled by the descriptor {1}", @object.GetType().FullName, this.GetType().FullName));
            }

            public virtual string Describe(object @object)
            {
                checkAndThrow(@object);
                var complex = (ComplexPropertyLevel1)@object;
                return complex.StringValue + "ComplexPropertyDescriptor";
            }
        }

        public class ComplexPropertyLevel2Descriptor: ComplexPropertyDescriptor
        {
            public ComplexPropertyLevel2Descriptor(Type connectToType): base(connectToType){}
            public override string Describe(object @object)
            {
                checkAndThrow(@object);
                var complex2 = (ComplexPropertyLevel2)@object;
                return complex2.StringValue + "ComplexPropertyLevel2Descriptor";
            }
        }
    }
}
