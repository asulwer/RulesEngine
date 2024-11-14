using RulesEngine.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Xunit;

namespace RulesEngine.UnitTest.Serialization
{
    public class ObjectAsPrimitiveConverterTests
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions {
            Converters = { new ObjectAsPrimitiveConverter() }
        };

        [Fact]
        public void Read_Null_ReturnsNull()
        {
            var json = "null";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.Null(result);
        }

        [Fact]
        public void Read_True_ReturnsTrue()
        {
            var json = "true";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.True((bool)result);
        }

        [Fact]
        public void Read_Integer_ReturnsInt32()
        {
            var json = "123";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.Equal(123, result);
        }

        [Fact]
        public void Read_LargeInteger_ReturnsInt64()
        {
            var json = "123456789012345";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            Assert.Equal(123456789012345L, result);
        }

        [Fact]
        public void Read_DecimalFormat_ReturnsDecimal()
        {
            var json = "123.45";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter(FloatFormat.Decimal, UnknownNumberFormat.Error, ObjectFormat.Expando);

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

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
        public void Read_Object_ReturnsExpandoObject()
        {
            var json = "{\"key\":\"value\"}";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

            reader.Read();
            var converter = new ObjectAsPrimitiveConverter();

            var result = converter.Read(ref reader, typeof(object), _jsonSerializerOptions);

            var dict = Assert.IsType<ExpandoObject>(result);
            Assert.Equal("value", ((IDictionary<string, object>)dict)["key"]);
        }
    }


}