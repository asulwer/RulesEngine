// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using RulesEngine.HelperFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;

namespace RulesEngine.Models
{
    [ExcludeFromCodeCoverage]
    public static class RuleParameterExtensions
    {
        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        public static ExpandoObject ToExpando(this object obj)
        {
            // Null-check
            Dictionary<string, object> expando = new Dictionary<string, object>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                expando.Add(property.Name, property.GetValue(obj));
            }

            return expando.ToExpando();
        }
    }

    [ExcludeFromCodeCoverage]
    public class RuleParameter
    {
        public RuleParameter(string name, object value)
        {
            Value = Utils.GetTypedObject(value);
            Init(name, Value?.GetType());
        }
       
        internal RuleParameter(string name, Type type,object value = null)
        {
            Value = Utils.GetTypedObject(value);
            Init(name, type);
        }

        public Type Type { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set; }
        public ParameterExpression ParameterExpression { get; private set; }

        private void Init(string name, Type type)
        {
            Name = name;
            Type = type ?? typeof(object);
            ParameterExpression = Expression.Parameter(Type, Name);
        }

        public static RuleParameter Create<T>(string name, T value)
        {
            var typedValue = Utils.GetTypedObject(value);
            var type = typedValue?.GetType() ?? typeof(T);
            return new RuleParameter(name,type,value);
        }        
    }
}
