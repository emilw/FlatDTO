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

        static void generateCode()
        {
            var objectFullName = "TestObject";
            /*foreach (var property in properties)
                objectFullName = objectFullName + property.Name;*/

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(objectFullName), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
            var dllName = objectFullName + ".dll";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName);

            //Define the type
            var typeBuilder = moduleBuilder.DefineType(objectFullName, System.Reflection.TypeAttributes.Public, typeof(BaseTest));

            //public abstract object Map(object typeA, object typeB);
            var methodBuilder = typeBuilder.DefineMethod("Map", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot |
                                            System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig,
                                            typeof(object), new Type[] { typeof(object), typeof(object) });

            ILGenerator il = methodBuilder.GetILGenerator();

            il.DeclareLocal(typeof(TypeA));
            il.DeclareLocal(typeof(TypeB));
            //localbuilder.SetLocalSymInfo("SourceObject");

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, typeof(TypeA));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Castclass, typeof(TypeB));
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldloc_1);
            /*il.Emit(OpCodes.Callvirt, typeof(TypeB).GetMethod("get_StringB"));
            il.Emit(OpCodes.Callvirt,  typeof(TypeA).GetMethod("set_StringA"));*/

            il.Emit(OpCodes.Callvirt, typeof(TypeB).GetProperty("StringB").GetGetMethod());
            il.Emit(OpCodes.Callvirt,  typeof(TypeA).GetProperty("StringA").GetSetMethod());
            
            //il.Emit(OpCodes.)
            il.Emit(OpCodes.Ldstr, "The I.M implementation of C");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine",
                new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var type = typeBuilder.CreateType();

            assemblyBuilder.Save("TestObject.dll");

            var objectToRun = (BaseTest)Activator.CreateInstance(type);

            var hello = objectToRun.Map(new TypeA(), new TypeB() { StringB = "Mjau" });

        }
    }
}
