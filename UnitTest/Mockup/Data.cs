using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        //Advance data
        public static ComplexDataType[] GetComplexTwoLevelMockupData()
        {
            var data = new List<ComplexDataType>();
            var comp1 = new ComplexPropertyLevel2() { StringValue = "ComlpexLevel2" };
            var comp2 = new ComplexPropertyLevel1() { ComplexProperty = comp1, StringValue = "ComplexLevel1" };
            data.Add(new ComplexDataType() { StringValue = "prop1", DecimalValue = 1, IntegerValue = 2, ComplexProperty = comp2});
            data.Add(new ComplexDataType() { StringValue = "prop2", DecimalValue = 100, IntegerValue = 3, ComplexProperty = comp2 });

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

        public class ComplexDataType : FlatDataType
        {
            public ComplexPropertyLevel1 ComplexProperty { get; set; }
        }

        public class ComplexPropertyLevel1
        {
            public ComplexPropertyLevel2 ComplexProperty { get; set; }
            public string StringValue { get; set; }
        }

        public class ComplexPropertyLevel2
        {
            public string StringValue { get; set; }
        }
    }
}
