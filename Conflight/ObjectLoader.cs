﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Conflight
{
    public class ObjectLoader
    {
        public static void LoadStatic<T>(string content)
        {
            var root = Config.Parse(content) as DictNode;

            if (root != null)
            {
                var fieldLookup = typeof(T).GetFields().Where(f => f.IsStatic).ToDictionary(f => f.Name.ToUpper());

                foreach (var kv in root.Value)
                {
                    var key = kv.Key.ToUpper();

                    FieldInfo field; 
                    if (fieldLookup.TryGetValue(key, out field))
                    {
                        if (field.FieldType == typeof(string))
                        {
                            field.SetValue(null, root.GetStringValue(kv.Key));
                        }
                        else if (field.FieldType == typeof(int))
                        {
                            field.SetValue(null, root.GetIntValue(kv.Key));
                        }
                        else if (field.FieldType == typeof(double))
                        {
                            field.SetValue(null, root.GetDoubleValue(kv.Key));
                        }
                        else if (field.FieldType == typeof(List<string>))
                        {
                            field.SetValue(null, root.GetStringListValue(kv.Key));
                        }
                    }
                }

                FieldInfo flagField;
                if (fieldLookup.TryGetValue("USINGDEFAULTS", out flagField))
                {
                    flagField.SetValue(null, false);
                }
            }
        }

        public static T LoadInstance<T>(string content) where T : new()
        {
            return LoadInstance<T>(Config.Parse(content) as DictNode);
        }

        public static List<T> LoadInstanceList<T>(string content) where T : new()
        {
            var listNode = Config.Parse(content) as ListNode;

            if (listNode == null)
            {
                throw new InvalidOperationException("Tried to get a list out of conflight content that does not have a list root node.");
            }

            return listNode.Value.Select(v => (T)GetInstance(typeof(T), v as DictNode)).ToList();
        }

        private static readonly HashSet<Type> ValueTypes = new HashSet<Type> {typeof(string), typeof(int), typeof(double), typeof(bool)}; 

        private static T LoadInstance<T>(DictNode root) where T : new()
        {
            return (T)GetInstance(typeof(T), root);
        }

        private static object GetInstance(Type t, DictNode node)
        {
            var result = Activator.CreateInstance(t);

            var props = t.GetProperties().ToDictionary(p => p.Name);

            var dict = node.Value;

            foreach (var p in props.Where(p => dict.ContainsKey(p.Key)))
            {
                p.Value.SetValue(result, GetObject(p.Value.PropertyType, dict[p.Key]), new object[0]);
            }

            return result;
        }

        private static object GetObject(Type t, ParseNode n)
        {
            if (ValueTypes.Contains(t))
            {
                return GetValue(t, n as TextNode);
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = t.GetGenericArguments()[0];

                return GetList(itemType, n as ListNode);
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = t.GetGenericArguments()[0];
                var valueType = t.GetGenericArguments()[1];

                return GetDictionary(keyType, valueType, n as DictNode);
            }

            if (t.BaseType == typeof(Array))
            {
                return GetArray(t.GetElementType(), n as ListNode);
            }

            if (t.IsEnum)
            {
                return GetEnumValue(t, n as TextNode);
            }

            return GetInstance(t, n as DictNode);
        }

        private static object GetEnumValue(Type t, TextNode textNode)
        {
            return Enum.Parse(t, textNode.Value);
        }

        private static object GetArray(Type valueType, ListNode listNode)
        {
            var result = Array.CreateInstance(valueType, listNode.Value.Count);

            for (int i = 0; i < listNode.Value.Count; i++)
            {
                result.SetValue(GetObject(valueType, listNode.Value[i]), i);
            }

            return result;
        }

        private static object GetDictionary(Type keyType, Type valueType, DictNode dictNode)
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(new[] {keyType, valueType});
            var result = Activator.CreateInstance(dictType);

            foreach (var pair in dictNode.Value)
            {
                object key;
                if (keyType.IsEnum)
                {
                    key = GetEnumValue(keyType, new TextNode() {Value = pair.Key});
                }
                else if (keyType == typeof(string))
                {
                    key = pair.Key;
                }
                else
                {
                    throw new ArgumentException("Key type must be an enum or a string.", "keyType");
                }

                dictType.GetMethod("Add").Invoke(result, new[] { key, GetObject(valueType, pair.Value)});
            }

            return result;
        }

        private static object GetValue(Type t, TextNode n)
        {
            return Convert.ChangeType(n.Value, t);
        }

        private static object GetList(Type itemType, ListNode root)
        {
            var listType = typeof(List<>).MakeGenericType(new[] {itemType});
            var result = Activator.CreateInstance(listType);

            foreach (var itemNode in root.Value)
            {
                listType.GetMethod("Add").Invoke(result, new[] { GetObject(itemType, itemNode) });
            }

            return result;
        }
    }
}