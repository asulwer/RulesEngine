// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using Xunit;

using RulesEngine.HelperFunctions;
using RulesEngine.Serialization;
using RulesEngine.Models;
using System.Text.Json.Nodes;

namespace RulesEngine.UnitTest
{
    [ExcludeFromCodeCoverage]
    public class TestClass
    {
        public string Test { get; set; }
        public List<int> TestList { get; set; }
    }

    [Trait("Category", "Unit")]
    [ExcludeFromCodeCoverage]
    public class UtilsTests
    {
        [Fact]
        public void GetTypedObject_dynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = "hello";
            obj.TestList = new List<int> { 1, 2, 3 };
            object typedobj = Utils.GetTypedObject(obj);
            Assert.IsNotType<ExpandoObject>(typedobj);
            Assert.NotNull(typedobj.GetType().GetProperty("Test"));
        }

        [Fact]
        public void GetTypedObject_Anonymous_dynamicObject()
        {
            dynamic obj = new ExpandoObject();
            ((IDictionary<string, object>)obj)["Item"] = "hello";
            ((IDictionary<string, object>)obj)["L"] = 1000;
            ((IDictionary<string, object>)obj)["W"] = 500;

            object typedobj = Utils.GetTypedObject(obj);
            Assert.IsNotType<ExpandoObject>(typedobj);
            Assert.NotNull(typedobj.GetType().GetProperty("L"));
            Assert.NotNull(typedobj.GetType().GetProperty("W"));
            Assert.NotNull(typedobj.GetType().GetProperty("Item"));
        }

        [Fact]
        public void GetTypedObject_dynamicObject_multipleObjects()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = "hello";
            obj.TestList = new List<int> { 1, 2, 3 };
            dynamic obj2 = new ExpandoObject();
            obj2.Test = "world";
            obj2.TestList = new List<int> { 1, 2, 3 };
            object typedobj = Utils.GetTypedObject(obj);
            object typedobj2 = Utils.GetTypedObject(obj2);
            Assert.IsNotType<ExpandoObject>(typedobj);
            Assert.NotNull(typedobj.GetType().GetProperty("Test"));
            Assert.Equal(typedobj.GetType(), typedobj2.GetType());
        }


        [Fact]
        public void GetTypedObject_nonDynamicObject()
        {
            var obj = new {
                Test = "hello"
            };
            var typedobj = Utils.GetTypedObject(obj);
            Assert.IsNotType<ExpandoObject>(typedobj);
            Assert.NotNull(typedobj.GetType().GetProperty("Test"));
        }


        [Fact]
        public void GetJsonElement_nonDynamicObject()
        {
            dynamic obj = JsonSerializer.SerializeToElement(new {
                Test = "hello"
            });
            dynamic typedobj = Utils.GetTypedObject(obj);
            Assert.IsNotType<ExpandoObject>(typedobj);
            Assert.IsType<JsonElement>(typedobj);
            //Assert.NotNull(((JsonElement)typedobj).GetProperty("Test")); //generates a warning
        }

        [Fact]
        public void CreateObject_dynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = "test";
            obj.TestList = new List<int> { 1, 2, 3 };

            object newObj = Utils.CreateObject(typeof(TestClass), obj);
            Assert.IsNotType<ExpandoObject>(newObj);
            Assert.NotNull(newObj.GetType().GetProperty("Test"));
        }

        [Fact]
        public void CreateAbstractType_dynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = "test";
            obj.TestList = new List<int> { 1, 2, 3 };
            obj.testEmptyList = new List<object>();

            Type type = Utils.CreateAbstractClassType(obj);
            Assert.NotEqual(typeof(ExpandoObject), type);
            Assert.NotNull(type.GetProperty("Test"));
        }

        [Fact]
        public void CreateAbstractClassType_WithJsonElement_ShouldConvertToExpandoObject()
        {
            const string jsonString = @"{""name"":""John"", ""age"":30, ""isStudent"":false}";
            var document = JsonSerializer.Deserialize<ExpandoObject>(jsonString);

            var type = Utils.CreateAbstractClassType(document);
            var propertyNames = type.GetProperties().Select(p => p.Name).ToArray();

            Assert.Contains("name", propertyNames);
            Assert.Contains("age", propertyNames);
            Assert.Contains("isStudent", propertyNames);
        }

        [Fact]
        public void CreateObject_WithJsonElement_ShouldConvertToExpandoObject()
        {
            var jsonString = @"{""name"":""John"", ""age"":30, ""isStudent"":false}";
            var document = JsonSerializer.Deserialize<ExpandoObject>(jsonString);

            var type = Utils.CreateAbstractClassType(document);
            dynamic result = Utils.CreateObject(type, document);

            Assert.Equal("John", result.name);
            Assert.Equal(30, result.age);
            Assert.False(result.isStudent);
        }

        [Fact]
        public void CreateObject_WithJsonElementNested_ShouldConvertToExpandoObject()
        {
            var jsonString = @"{""name"":""John"", ""details"":{""age"":30, ""isStudent"":false}}";
            var document = JsonSerializer.Deserialize<ExpandoObject>(jsonString);

            var type = Utils.CreateAbstractClassType(document);
            dynamic result = Utils.CreateObject(type, document);

            Assert.Equal("John", result.name);
            Assert.Equal(30, result.details.age);
            Assert.False(result.details.isStudent);
        }

        [Fact]
        public void CreateObject_WithJsonElementArray_ShouldConvertToExpandoObject()
        {
            const string jsonString = @"{""name"":""John"", ""scores"":[100, 95.7, 85]}";
            var document = JsonSerializer.Deserialize<ExpandoObject>(jsonString);


            var type = Utils.CreateAbstractClassType(document);
            dynamic result = Utils.CreateObject(type, document);

            Assert.Equal("John", result.name);

            var scores = (List<object>)result["scores"];
            Assert.Equal(100L, scores[0]);
            Assert.Equal(95.7, scores[1]);
            Assert.Equal(85L, scores[2]);
        }

        [Fact]
        public void CreateObject_WithEmptyArray_ShouldConvertToListDictionaryImplicitObject()
        {
            var jsonString = @"{""things"":[]}";

            var options = new JsonSerializerOptions {
                Converters = { new ObjectAsPrimitiveConverter() },
                WriteIndented = true,
            };

            var document = JsonSerializer.Deserialize<ExpandoObject>(jsonString, options);

            var type = Utils.CreateAbstractClassType(document);
            dynamic result = Utils.CreateObject(type, document);

            Assert.IsType<List<Dictionary<string, ImplicitObject>>>(result.things);
            Assert.Empty((List<Dictionary<string, ImplicitObject>>)result.things);
        }
    }
}