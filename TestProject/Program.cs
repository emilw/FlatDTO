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

            var mapperEngine = new FlatDTO.FlatMapperEngine();

            var start = DateTime.Now;
            factory.Create<MyBaseDTO>(data[0].GetType(), new string[] { "Property", "Thingy", "Prio" }, mapperEngine);
            var stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks)/10000);
            start = DateTime.Now;
            factory.Create<MyBaseDTO>(data[0].GetType(), new string[] { "Property", "Thingy", "Prio" }, mapperEngine);
            stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks) / 10000);
            start = DateTime.Now;
            factory.Create<MyBaseDTO>(data[0].GetType(), new string[] { "Property", "Thingy" }, mapperEngine);
            stop = DateTime.Now;
            Console.WriteLine((stop.Ticks - start.Ticks) / 10000);

            Console.ReadLine();

            //var dto2 = factory.Create(data.ToArray(), new string[] { "Property1" });

            //var dto3 = factory.Create(data.ToArray(), new string[] { "Property" });
            //generateCode();
        }
    }
}
