using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.TestFolder
{
    public class TestClass
    {
        private List<Flat> Map(List<Complex> complex, List<Flat> flat)
        {
            foreach (var flatItem in flat)
            {
                foreach (var complexItem in complex)
                {
                    flatItem.Value = complexItem.Value;
                }
            }

            return flat;
        }

        private List<Flat> MapList(List<ComplexLine> complex, List<Flat> flat)
        {
            //foreach (var flatItem in flat)
            //{
                foreach (var complexItem in complex)
                {
                    var flatItem = new Flat();
                    flatItem.ListValue = complexItem.ListValue;
                    flat.Add(flatItem);
                }
            //}B
                return flat;
        }
    }

    public class Flat
    {
        public String Value { get; set; }
        public String ListValue { get; set; }
    }

    public class Complex
    {
        public static Complex getComplex()
        {
            var complex = new Complex();
            complex.Value = "DudeHeader";
            complex.List = new List<ComplexLine>();
            complex.List.Add(new ComplexLine() { ListValue = "1" });
            complex.List.Add(new ComplexLine() { ListValue = "2" });
            complex.List.Add(new ComplexLine() { ListValue = "3" });
            complex.List.Add(new ComplexLine() { ListValue = "4" });
            return complex;
        }
        public String Value { get; set; }
        public List<ComplexLine> List { get; set; }
    }
    public class ComplexLine
    {
        public String ListValue { get; set; }
    }
}
