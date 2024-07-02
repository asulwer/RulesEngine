using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace RulesEngine.HelperFunctions
{
    public static class JsonElementExtensions
    {
        public static dynamic ToExpandoObject(this JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var expandoObject = new ExpandoObject() as IDictionary<string, object>;
                    foreach (var property in element.EnumerateObject())
                    {
                        expandoObject[property.Name] = property.Value.ToExpandoObject();
                    }
                    return expandoObject;

                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(item.ToExpandoObject());
                    }
                    return list;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    return GetNumberValue(element);

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Null:
                    return null;

                default:
                    return null;
            }
        }

        private static object GetNumberValue(JsonElement element)
        {
            if (element.TryGetInt64(out var longValue))
            {
                return longValue;
            }
            if (element.TryGetDouble(out var doubleValue))
            {
                return doubleValue;
            }

            return element.GetDecimal();
        }
    }
}