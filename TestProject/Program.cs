using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var data = new List<DummyClass>();
            data.Add(new DummyClass(){Property="prop1", Prio=1, Thingy="Thiss"});
            data.Add(new DummyClass(){Property="prop2", Prio=100, Thingy="dkdkkd"});

            var factory = new FlatDTO.FlatDTOFactory();

            var start = DateTime.Now;
            factory.Create<MyBaseDTO>(data.ToArray(), new string[] { "Property", "Thingy", "Prio" });
            var stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks)/10000);
            start = DateTime.Now;
            factory.Create<MyBaseDTO>(data.ToArray(), new string[] { "Property", "Thingy", "Prio" });
            stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks) / 10000);
            start = DateTime.Now;
            factory.Create<MyBaseDTO>(data.ToArray(), new string[] { "Property", "Thingy"});
            stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks) / 10000);

            Console.ReadLine();

            //var dto2 = factory.Create(data.ToArray(), new string[] { "Property1" });

            //var dto3 = factory.Create(data.ToArray(), new string[] { "Property" });
            //generateCode();
        }
    }
}
