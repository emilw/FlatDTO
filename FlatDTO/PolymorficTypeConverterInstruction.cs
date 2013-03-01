using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlatDTO
{
    public class PolymorficTypeConverterInstruction
    {
        public PolymorficTypeConverterInstruction(string propertyPath, Type convertToType)
        {
            PropertyPath = propertyPath;
            ConvertToType = convertToType;
        }
        public string PropertyPath { get; set; }
        public Type ConvertToType { get; set; }
    }
}
