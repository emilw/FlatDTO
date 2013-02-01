using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.Mockup
{
    public static class Data
    {
        public static DummyClass[] GetSimpleOneLevelMockupData()
        {
            var data = new List<DummyClass>();
            data.Add(new DummyClass() { StringValue = "prop1", DecimalValue = 1, IntegerValue = 2 });
            data.Add(new DummyClass() { StringValue = "prop2", DecimalValue = 100, IntegerValue = 3 });

            return data.ToArray();
        }

        public static string[] GetSimpleOneLevelMockupDataPropertyStrings()
        {
            var stringPropertyName = "StringValue";
            var decimalPropertyName = "DecimalValue";
            var integerPropertyName = "IntegerValue";

            return new string[] { stringPropertyName, decimalPropertyName, integerPropertyName };
        }

        public class DummyClass
        {
            public string StringValue { get; set; }
            public decimal DecimalValue { get; set; }
            public int IntegerValue { get; set; }

        }

        public class MyDTO
        {

        }
    }
}
