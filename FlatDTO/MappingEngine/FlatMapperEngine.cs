using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace FlatDTO
{
    public class FlatMapperEngine : MapperEngine
    {
        private string _typeName;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;


        private ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_assemblyBuilder == null)
                {
                    _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName(TypeName), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
                    _moduleBuilder = _assemblyBuilder.DefineDynamicModule(TypeName + ".dll");
                }

                return _moduleBuilder;
            }
        }
        
        protected override BaseClass.DTOMapper<T> CreateMapper<T>(Type sourceType, Type destinationType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            var objectFullName = TypeName + "MAPPER";

            var moduleBuilder = ModuleBuilder;

            //Define the type
            var typeBuilder = moduleBuilder.DefineType(objectFullName, System.Reflection.TypeAttributes.Public, typeof(BaseClass.DTOMapper<T>));

            var unMapMethodBuilder = typeBuilder.DefineMethod("Map", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.ReuseSlot |
                                            System.Reflection.MethodAttributes.Virtual | System.Reflection.MethodAttributes.HideBySig,
                                            typeof(object), new Type[] { typeof(object), typeof(object) });


            ILGenerator il = unMapMethodBuilder.GetILGenerator();

            GenerateMapperHeader(il, sourceType, destinationType);

            GeneratePropertyMaps(il, destinationType, sourceType, properties);

            GenerateFooter(il);

            var type = typeBuilder.CreateType();

            var objectToRun = (BaseClass.DTOMapper<T>)Activator.CreateInstance(type);

            objectToRun.SourceType = sourceType;
            objectToRun.DestinationType = destinationType;

            return objectToRun;
        }

        private void GenerateMapperHeader(ILGenerator il, Type sourceType, Type destinationType)
        {
            il.DeclareLocal(sourceType);
            il.DeclareLocal(destinationType);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Castclass, destinationType);
            il.Emit(OpCodes.Stloc_1);
        }

        private void GenerateFooter(ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }

        private void GeneratePropertyMaps(ILGenerator il, Type destinationType, Type sourceType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            //Get the properties to use
            foreach (var propertyList in properties)
            {
                //Load the source type
                il.Emit(OpCodes.Ldloc_1);
                //Load the destination type
                il.Emit(OpCodes.Ldloc_0);

                //Go through the object tree for the complex type
                foreach (var property in propertyList.Item2)
                {
                    //Get each get method
                    il.Emit(OpCodes.Callvirt, property.SystemProperty.GetGetMethod());
                    //Convert the type if it's marked as polymorfic
                    if (property.IsPolyMorfic)
                        il.Emit(OpCodes.Castclass, property.Type);
                }
                //Set the value
                il.Emit(OpCodes.Callvirt, destinationType.GetProperty(propertyList.Item1).GetSetMethod());
            }
        }

        private void GeneratePropertyUnMaps(ILGenerator il, Type destinationType, Type sourceType, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            //Get the properties to use
            foreach (var propertyList in properties)
            {
                //Load the source type
                il.Emit(OpCodes.Ldloc_1);
                //Load the destination type
                il.Emit(OpCodes.Ldloc_0);

                //Set the value
                il.Emit(OpCodes.Callvirt, destinationType.GetProperty(propertyList.Item1).GetGetMethod());

                //Go through the object tree for the complex type
                foreach (var property in propertyList.Item2)
                {
                    //Set the value if it's the last property = simple type
                    if (IsSimpleType(property.SystemProperty.PropertyType))
                    {
                        il.Emit(OpCodes.Callvirt, property.SystemProperty.GetSetMethod());
                    }
                    else
                    {
                        //Get each get method
                        il.Emit(OpCodes.Callvirt, property.SystemProperty.GetGetMethod());
                        //Convert the type if it's marked as polymorfic
                        if (property.IsPolyMorfic)
                            il.Emit(OpCodes.Castclass, property.Type);
                    }
                }
            }
        }


        protected override Type CreateDTOType<T>(Type type, List<Tuple<string, List<PropertyInfoEx>>> properties)
        {
            var objectFullName = TypeName;

            var moduleBuilder = ModuleBuilder;

            //Define the type
            var typeBuilder = moduleBuilder.DefineType(objectFullName, System.Reflection.TypeAttributes.Public, typeof(T));

            var baseTypeProperties = typeof(T).GetProperties();

            //Go through all the parameters
            foreach (var prop in properties)
            {
                //If the property already exists in the base type, it should not be added
                if(!baseTypeProperties.Any(x => string.Equals(x.Name, prop.Item1, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var attributes = new List<KeyValuePair<Type, object[]>>();
                    attributes.Add(new KeyValuePair<Type, object[]>(typeof(System.Runtime.Serialization.DataMemberAttribute), new object[]{}));
                    //Build the property
                    buildProperty(typeBuilder, prop.Item1, prop.Item2.Last().Type, attributes);
                }
            }

            //Create the type
            var newType = typeBuilder.CreateType();

            //assemblyBuilder.Save(dllName);

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
