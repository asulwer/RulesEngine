using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using RulesEngine.Serialization;
using Xunit;

namespace RulesEngine.UnitTest.Serialization
{
    public class ObjectAsPrimitiveConverterTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ObjectAsPrimitiveConverterTests()
        {
            _jsonSerializerOptions = new JsonSerializerOptions {
                Converters = { new ObjectAsPrimitiveConverter() }
            };
        }

        [Theory]
        [InlineData(JsonTokenType.Null, null)]
        [InlineData(JsonTokenType.False, false)]
        [InlineData(JsonTokenType.True, true)]
        [InlineData(JsonTokenType.String, "test")]
        public void Read_PrimitiveValues_ReturnsExpected(JsonTokenType tokenType, object expected)
        {
            var json = tokenType == JsonTokenType.String ? "\"test\"" : expected?.ToString().ToLower() ?? "null";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Read_Integer_ReturnsInt32OrInt64()
        {
            var json = "123";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.IsType<int>(result);
            Assert.Equal(123, result);
        }

        [Fact]
        public void Read_Float_ReturnsExpectedFloatType()
        {
            var json = "123.45";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter(FloatFormat.Decimal, UnknownNumberFormat.Error, ObjectFormat.Expando);

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.IsType<decimal>(result);
            Assert.Equal(123.45m, result);
        }

        [Fact]
        public void Read_Array_ReturnsList()
        {
            var json = "[1, 2, 3]";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.IsType<List<object>>(result);
            Assert.Equal(new List<object> { 1, 2, 3 }, result);
        }

        [Fact]
        public void Read_Object_ReturnsDictionary()
        {
            var json = "{\"key\":\"value\"}";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.IsType<ExpandoObject>(result);
            Assert.Equal("value", ((IDictionary<string, object>)result)["key"]);
        }

        [Fact]
        public void Write_ObjectType_WritesEmptyObject()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectAsPrimitiveConverter());
            var converter = new ObjectAsPrimitiveConverter();
            var value = new object();

            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, value, options);
            writer.Flush();

            var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal("{}", json);
        }

        [Fact]
        public void Write_PrimitiveType_WritesValue()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectAsPrimitiveConverter());
            var converter = new ObjectAsPrimitiveConverter();
            var value = 123;

            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, value, options);
            writer.Flush();

            var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal("123", json);
        }

        [Fact]
        public void Read_UnknownNumberFormatJsonElement_ReturnsJsonElement()
        {
            var json = "12345678901234567890";  // large number
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter(FloatFormat.Double, UnknownNumberFormat.JsonElement, ObjectFormat.Expando);

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.IsType<double>(result);
        }

        [Fact]
        public void Read_NestedJsonObject_ReturnsNestedDictionary()
        {
            var json = "{\"outer\":{\"inner\":\"value\"}}";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            var outerDict = Assert.IsType<ExpandoObject>(result);
            var innerDict = Assert.IsType<ExpandoObject>(((IDictionary<string, object>)outerDict)["outer"]);
            Assert.Equal("value", ((IDictionary<string, object>)innerDict)["inner"]);
        }

        [Fact]
        public void Read_EmptyArray_ReturnsEmptyList()
        {
            var json = "[]";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            var list = Assert.IsType<List<object>>(result);
            Assert.Empty(list);
        }

        [Fact]
        public void Read_EmptyObject_ReturnsEmptyDictionary()
        {
            var json = "{}";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            var dict = Assert.IsType<ExpandoObject>(result);
            Assert.Empty((IDictionary<string, object>)dict);
        }

        [Fact]
        public void Read_PropertyNameWithSpecialCharacters_ReturnsExpected()
        {
            var json = "{\"property name!\":\"value\"}";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            var dict = Assert.IsType<ExpandoObject>(result);
            Assert.Equal("value", ((IDictionary<string, object>)dict)["property name!"]);
        }
    }
}