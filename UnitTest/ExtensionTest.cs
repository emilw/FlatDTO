using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatDTO;
using System.Linq.Expressions;
using FlatDTO.Exception;

namespace UnitTest
{
    [TestClass]
    public class ExtensionTest
    {

        [TestMethod]
        public void DummyMapperExtensionTest()
        {
            var factory = new FlatDTO.FlatDTOFactory();
            var properties = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();
            var data = Mockup.Data.GetSimpleOneLevelMockupData();
            var mapper = factory.Create<Mockup.Data.FlatDataType>(data[0].GetType(), properties, new Mockup.DummyMapperFlatEngineExtension());

            Assert.IsTrue(mapper.DestinationType.FullName.Contains("mjau"));
        }

        [TestMethod]
        public void UseCustomMapperToVerify1ObjectToMany()
        {
            var factory = new FlatDTO.FlatDTOFactory();
            var properties = Mockup.Data.GetSimpleOneLevelMockupDataPropertyStrings();
            var data = Mockup.Data.GetSimpleOneLevelMockupData();
            var mapper = factory.Create<Mockup.Data.FlatDataType>(data[0].GetType(), properties,
                                                                    new Mockup.DummyMapperEngine());

            //Data records sent in is 2
            var result = mapper.Map(data);
            //Result is 22, the DummyMapperEngine uses another mapper that returns static 10 items per item sent in
            Assert.IsTrue(result.Count() > 21);
        }

        [TestMethod]
        public void TestExpressionTree()
        {
            var parameter = Expression.Parameter(typeof(int), "arg");
            var method = MethodCallExpression.Call(typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }), parameter);

            var action = Expression.Lambda<Action<int>>(method, new ParameterExpression[] { parameter }).Compile();

            action(4);
        }

        [TestMethod]
        public void TestExpressionTree2()
        {
            var parameter = Expression.Parameter(typeof(int), "value");
            var result = Expression.Parameter(typeof(int), "result");

            var label = Expression.Label(typeof(int));
            var block = Expression.Block(new[] { result },
                                            Expression.Assign(result, Expression.Constant(1)),
                                            Expression.Loop(
                                                Expression.IfThenElse(
                                                                Expression.LessThan(result, Expression.Constant(10)),
                                                                Expression.AddAssign(result, Expression.Constant(1)),
                                                                Expression.Break(label, result)),
                                                           label)
                                                   );
            var action = Expression.Lambda<Func<int, int>>(block, parameter).Compile();

            
            var outValue = action(0);

            Assert.IsTrue(outValue == 10);
        }

        [TestMethod]
        public void TestExpressionTree3()
        {

            var source = TestFolder.Complex.getComplex();

            var parameter = Expression.Parameter(source.GetType(), "value");
            var result = Expression.Parameter(typeof(int), "result");
            //var variable = Expression.Variable(source.List.GetType());
            var property = Expression.Property(parameter, "List");
            //Expression.Assign(variable, Expression.Property(parameter,"List"));
            var itemCount = Expression.Call(property,property.Type.GetMethod("get_Count"));
            
            var label = Expression.Label(typeof(int));
            var block = Expression.Block(new[] { result },
                                            Expression.Assign(result, Expression.Constant(1)),
                                            Expression.Loop(
                                                Expression.IfThenElse(
                                                                //Expression.GetMet
                                                                Expression.LessThan(result, itemCount),
                                                                Expression.AddAssign(result, Expression.Constant(1)),
                                                                Expression.Break(label, result)),
                                                           label)
                                                   );
            var action = Expression.Lambda<Func<TestFolder.Complex, int>>(block, parameter).Compile();


            var outValue = action(source);

            Assert.IsTrue(outValue == 4);
        }

        [TestMethod]
        public void FlatMapperEngineWithRepeaterTestRegularRepeater()
        {
            var factory = new FlatDTO.FlatDTOFactory();
            var properties = Mockup.Data.GetComplexTwoLevelListMockupDataPropertyStrings();
            var data = Mockup.Data.GetComplexTwoLevelMockupData();
            var mapper = factory.Create<Mockup.Data.FlatDataType>(data[0].GetType(), properties,
                                                                    new FlatDTO.MappingEngine.FlatMapperEngineWithRepeater());

            //Data records sent in is 2
            var result = mapper.Map(data);
            //Result is 22, the DummyMapperEngine uses another mapper that returns static 10 items per item sent in
            Assert.IsTrue(result.Count() == 4);

            var property = result[0].GetType().GetProperty("ComplexPropertyList_ComplexProperty_StringValue");
            var value = ((string)property.GetValue(result[0], null));

            Assert.IsTrue(String.Equals(value, "LineComplexValue1"));

            property = result[0].GetType().GetProperty("ComplexPropertyList_StringValue");
            value = ((string)property.GetValue(result[0], null));

            Assert.IsTrue(String.Equals(value, "Line1"));
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleListItemInstructionsException))]
        public void FlatMapperEngineWithRepeaterTestWithMultipleLineInstructions()
        {
            var factory = new FlatDTO.FlatDTOFactory();
            var properties = Mockup.Data.GetComplexTwoLevelListMockupDataPropertyStrings().ToList();

            properties.Add("ComplexPropertyList2.ComplexProperty.StringValue");
            var data = Mockup.Data.GetComplexTwoLevelMockupData();
            var mapper = factory.Create<Mockup.Data.FlatDataType>(data[0].GetType(), properties.ToArray(),
                                                                    new FlatDTO.MappingEngine.FlatMapperEngineWithRepeater());
            var result = mapper.Map(data);
        }
    }
}
