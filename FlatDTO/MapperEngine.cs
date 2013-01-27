using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace FlatDTO
{
    public static class MapperEngine
    {
        public static FlatDTO.BaseClass.DTOMapper CreateMapper(Type type, string[] properties)
        {
            var propertyInfos = GetPropertiesToUse(type, properties);

            var destinationType = CreateDTOType(type, propertyInfos);

            var mapper = CreateMapper(type, destinationType, propertyInfos);

            return mapper;
        }

        private static BaseClass.DTOMapper CreateMapper(Type sourceType, Type destinationType, PropertyInfo[] properties)
        {
            var objectFullName = destinationType.FullName + "MAPPER";

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(objectFullName), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
            var dllName = objectFullName + ".dll";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName);

            //Define the type
            var typeBuilder = moduleBuilder.DefineType(objectFullName, System.Reflection.TypeAttributes.Public, typeof(BaseClass.DTOMapper));

            //public abstract object Map(object typeA, object typeB);
            var methodBuilder = typeBuilder.DefineMethod("Map", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot |
                                            System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig,
                                            typeof(BaseClass.DTO), new Type[] { typeof(object), typeof(BaseClass.DTO) });

            ILGenerator il = methodBuilder.GetILGenerator();

            il.DeclareLocal(sourceType);
            il.DeclareLocal(destinationType);
            //localbuilder.SetLocalSymInfo("SourceObject");

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Castclass, destinationType);
            il.Emit(OpCodes.Stloc_1);
            /*il.Emit(OpCodes.Callvirt, typeof(TypeB).GetMethod("get_StringB"));
            il.Emit(OpCodes.Callvirt,  typeof(TypeA).GetMethod("set_StringA"));*/

            foreach (var property in properties)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldloc_0);
                /*il.Emit(OpCodes.Callvirt, sourceType.GetProperty("Property").GetGetMethod());
                il.Emit(OpCodes.Callvirt, destinationType.GetProperty("Property").GetSetMethod());*/
                il.Emit(OpCodes.Callvirt, property.GetGetMethod());
                il.Emit(OpCodes.Callvirt, destinationType.GetProperty(property.Name).GetSetMethod());

            }

            //il.Emit(OpCodes.)
            il.Emit(OpCodes.Ldstr, "The I.M implementation of C");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine",
                new Type[] { typeof(string) }));

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            var type = typeBuilder.CreateType();

            assemblyBuilder.Save(dllName);

            var objectToRun = (BaseClass.DTOMapper)Activator.CreateInstance(type);

            objectToRun.SourceType = sourceType;
            objectToRun.DestinationType = destinationType;

            return objectToRun;
            //var hello = objectToRun.Map(new TypeA(), new TypeB() { StringB = "Mjau" });

        }

        private static System.Reflection.PropertyInfo[] GetPropertiesToUse(Type type, string[] propertiesToUse)
        {
            var properties = type.GetProperties();
            var result = new List<System.Reflection.PropertyInfo>();

            foreach (var property in properties)
            {
                if (propertiesToUse.Where(x => x == property.Name).FirstOrDefault() != null)
                    result.Add(property);
            }

            return result.ToArray();
        } 


        private static Type CreateDTOType(Type type, PropertyInfo[] properties)
        {
            var objectFullName = type.FullName;
            foreach (var property in properties)
                objectFullName = objectFullName + property.Name;

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(objectFullName), AssemblyBuilderAccess.RunAndSave);
            var dllName = objectFullName + ".dll";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(dllName);

            //Define the type
            var typeBuilder = moduleBuilder.DefineType(objectFullName, System.Reflection.TypeAttributes.Public, typeof(FlatDTO.BaseClass.DTO));
            //var typeBuilder = _moduleBuilder.DefineType(_dataObjectType.FullName, TypeAttributes.Public);
            //Set the parent type to the type
            //typeBuilder.SetParent(this.resolveType(_dataObjectType.ParentType.FullName));

            //Go through all the parameters
            foreach (var prop in properties)
            {
                var attributes = new List<KeyValuePair<Type, object[]>>();

                //Build the property
                buildProperty(typeBuilder, prop.Name, prop.PropertyType, attributes);
            }

            //Create the type
            var newType = typeBuilder.CreateType();

            assemblyBuilder.Save(dllName);

            return newType;
        }

        private static void buildProperty(TypeBuilder typeBuilder, string name, Type type, List<KeyValuePair<Type, object[]>> attributes)
        {
            //var propBuilder = typeBuilder.DefineProperty(name, System.Reflection.PropertyAttributes.HasDefault, type, null);
            var propBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, new Type[] { });

            FieldBuilder customerNameBldr = typeBuilder.DefineField(name.ToLower(),
                                                        type,
                                                        FieldAttributes.Private);


            MethodAttributes getSetAttr =
            MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder custNameGetPropMthdBldr =
                typeBuilder.DefineMethod("get_"+name,
                                           getSetAttr,
                                           type,
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder custNameSetPropMthdBldr =
                typeBuilder.DefineMethod("set_"+name,
                                           getSetAttr,
                                           null,
                                           new Type[] { type });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively.
            propBuilder.SetGetMethod(custNameGetPropMthdBldr);
            propBuilder.SetSetMethod(custNameSetPropMthdBldr);

            foreach (var attribute in attributes)
            {
                //Type[] ctorParams = new Type[] {};
                Type[] ctorParams = attribute.Value.Select(x => x.GetType()).ToArray();
                ConstructorInfo classCtorInfo =
                attribute.Key.GetConstructor(ctorParams);

                CustomAttributeBuilder myCABuilder = new CustomAttributeBuilder(
                classCtorInfo, attribute.Value);
                //new object[] { "This is the long description of this property." });

                propBuilder.SetCustomAttribute(myCABuilder);
                //propBuilder.SetCustomAttribute()
            }

        }
    }
}
