using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
    public class ExampleClass : classNum2
    {
        public string Hello { get; set; }

        public void Map(object @object)
        {
            var innerClass = new ExampleClass();
            var myClass = (classNum2)@object;

            innerClass.Hello = myClass.Test;

        }
    }

    public class classNum2
    {
        public string Test { get; set; }
    }
}
