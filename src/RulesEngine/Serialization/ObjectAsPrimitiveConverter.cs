﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// https://github.com/asulwer/RulesEngine/issues/75
// https://stackoverflow.com/questions/65972825/c-sharp-deserializing-nested-json-to-nested-dictionarystring-object

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RulesEngine.Serialization
{
    [ExcludeFromCodeCoverage]
    public class ObjectAsPrimitiveConverter : JsonConverter<object>
    {
        
        #region Fields

        private readonly FloatFormat floatFormat;
        private readonly UnknownNumberFormat unknownNumberFormat;
        private readonly ObjectFormat objectFormat;

        #endregion

        #region Constructors

        public ObjectAsPrimitiveConverter() : this(FloatFormat.Double, UnknownNumberFormat.Error, ObjectFormat.Expando) { }
        public ObjectAsPrimitiveConverter(FloatFormat floatFormat, UnknownNumberFormat unknownNumberFormat, ObjectFormat objectFormat)
        {
            this.floatFormat = floatFormat;
            this.unknownNumberFormat = unknownNumberFormat;
            this.objectFormat = objectFormat;
        }

        #endregion

        #region Methods

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value.GetType() == typeof(object))
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            }
            else
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    {
                        if (reader.TryGetInt32(out var i))
                            return i;
                        if (reader.TryGetInt64(out var l))
                            return l;
                        // BigInteger could be added here.
                        if (floatFormat == FloatFormat.Decimal && reader.TryGetDecimal(out var m))
                            return m;
                        else if (floatFormat == FloatFormat.Double && reader.TryGetDouble(out var d))
                            return d;
                        using var doc = JsonDocument.ParseValue(ref reader);
                        if (unknownNumberFormat == UnknownNumberFormat.JsonElement)
                            return doc.RootElement.Clone();
                        throw new JsonException(string.Format("Cannot parse number {0}", doc.RootElement.ToString()));
                    }
                case JsonTokenType.StartArray:
                    {
                        var list = new List<object>();
                        while (reader.Read())
                        {
                            switch (reader.TokenType)
                            {
                                default:
                                    list.Add(Read(ref reader, typeof(object), options));
                                    break;
                                case JsonTokenType.EndArray:
                                    return list;
                            }
                        }
                        throw new JsonException();
                    }
                case JsonTokenType.StartObject:
                    var dict = CreateDictionary();
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndObject:
                                return dict;
                            case JsonTokenType.PropertyName:
                                var key = reader.GetString();
                                reader.Read();
                                dict.Add(key, Read(ref reader, typeof(object), options));
                                break;
                            default:
                                throw new JsonException();
                        }
                    }
                    throw new JsonException();
                default:
                    throw new JsonException(string.Format("Unknown token {0}", reader.TokenType));
            }
        }

        protected virtual IDictionary<string, object> CreateDictionary()
        {
            return objectFormat == ObjectFormat.Expando ? new ExpandoObject() : (IDictionary<string, object>)new Dictionary<string, object>();
        }

        #endregion

    }

    public enum FloatFormat
    {
        Double,
        Decimal,
    }

    public enum UnknownNumberFormat
    {
        Error,
        JsonElement,
    }

    public enum ObjectFormat
    {
        Expando,
        Dictionary,
    }
}
