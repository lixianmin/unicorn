/********************************************************************
created:    2018-01-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using FieldInfo = System.Reflection.FieldInfo;
using BindingFlags = System.Reflection.BindingFlags;

namespace Unicorn
{
    public static class InspectorTools
    {
        private class MetaInfo
        {
            public bool foldout;
        }

        public static bool DrawObject(object target, string name)
        {
            if (null == target)
            {
                return false;
            }

            name ??= string.Empty;

            var type = target.GetType();
            bool foldout;
            if (type.IsValueType)
            {
                foldout = true;
                EditorGUILayout.LabelField(name);
            }
            else
            {
                var meta = _FetchMetaInfo(target, name);
                foldout = _EditorGUILayout_Foldout(meta.foldout, name);
                meta.foldout = foldout;
            }

            if (!foldout)
            {
                return false;
            }
            
            var isChanged = false;
            EditorGUI.indentLevel++;
            {
                var flags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var field in type.GetFields(flags))
                {
                    var isFieldChanged = _DrawField(target, field);
                    if (isFieldChanged)
                    {
                        isChanged = true;
                    }
                }
            }
            EditorGUI.indentLevel--;

            return isChanged;
        }

        private static bool _DrawField(object target, FieldInfo field)
        {
            var type = field.FieldType;
            var isChanged = false;

            if (type == typeof(bool))
            {
                isChanged = _DrawToggleField(target, field);
            }
            else if (type == typeof(byte))
            {
                isChanged = _DrawIntField<byte>(target, field);
            }
            else if (type == typeof(sbyte))
            {
                isChanged = _DrawIntField<sbyte>(target, field);
            }
            else if (type == typeof(short))
            {
                isChanged = _DrawIntField<short>(target, field);
            }
            else if (type == typeof(ushort))
            {
                isChanged = _DrawIntField<ushort>(target, field);
            }
            else if (type == typeof(int))
            {
                isChanged = _DrawIntField<int>(target, field);
            }
            else if (type == typeof(uint))
            {
                isChanged = _DrawLongField<uint>(target, field);
            }
            else if (type == typeof(long))
            {
                isChanged = _DrawLongField<long>(target, field);
            }
            else if (type == typeof(float))
            {
                isChanged = _DrawFloatField(target, field);
            }
            else if (type == typeof(double))
            {
                isChanged = _DrawDoubleField(target, field);
            }
            else if (type == typeof(char))
            {
                isChanged = _DrawCharField(target, field);
            }
            else if (type == typeof(string))
            {
                isChanged = _DrawTextField(target, field);
            }
            else if (type == typeof(Vector2))
            {
                isChanged = _DrawVector2Field(target, field);
            }
            else if (type == typeof(Vector3))
            {
                isChanged = _DrawVector3Field(target, field);
            }
            else if (type == typeof(Vector4))
            {
                isChanged = _DrawVector4Field(target, field);
            }
            else if (type == typeof(Color))
            {
                isChanged = _DrawColorField(target, field);
            }
            else if (type.IsArray)
            {
                isChanged = _DrawArrayField(target, field);
            }
            else if (type.IsEnum)
            {
                isChanged = _DrawEnumField(target, field);
            }
            else if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(List<>) == genericTypeDefinition)
                {
                    var elementType = type.GetGenericArguments()[0];
                    isChanged = _DrawListField(target, field, elementType);
                }
            }
            else if (type.IsValueType)
            {
                var val = field.GetValue(target);
                var name = field.Name;
                isChanged = DrawObject(val, name);
                if (isChanged)
                {
                    field.SetValue(target, val);
                }
            }
            else
            {
                var val = field.GetValue(target);
                if (null == val)
                {
                    val = _CreateInstance(field.FieldType);
                    field.SetValue(target, val);
                    isChanged = true;
                }

                var name = field.Name;
                isChanged = DrawObject(val, name) || isChanged;
            }

            return isChanged;
        }

        private static bool _DrawToggleField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (bool)field.GetValue(target);

            var result = EditorGUILayout.Toggle(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawIntField<T>(object target, FieldInfo field) where T : IConvertible
        {
            var label = field.Name;
            var val = Convert.ToInt32((T)field.GetValue(target));

            var result = EditorGUILayout.IntField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawLongField<T>(object target, FieldInfo field) where T : IConvertible
        {
            var label = field.Name;
            var val = Convert.ToInt64((T)field.GetValue(target));

            var result = EditorGUILayout.LongField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawFloatField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (float)field.GetValue(target);

            var result = EditorGUILayout.FloatField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawDoubleField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (double)field.GetValue(target);

            var result = EditorGUILayout.DoubleField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawCharField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = ((char)field.GetValue(target)).ToString();

            var result = EditorGUILayout.TextField(label, val);
            if (val != result)
            {
                var result0 = result[0];
                field.SetValue(target, result0);
                return true;
            }

            return false;
        }

        private static bool _DrawTextField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (string)field.GetValue(target);

            var result = EditorGUILayout.TextField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawVector2Field(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (Vector2)field.GetValue(target);

            var result = EditorGUILayout.Vector2Field(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawVector3Field(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (Vector3)field.GetValue(target);

            var result = EditorGUILayout.Vector3Field(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawVector4Field(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (Vector4)field.GetValue(target);

            var result = EditorGUILayout.Vector4Field(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawEnumField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (Enum)field.GetValue(target);

            var result = EditorGUILayout.EnumPopup(label, val);
            if (Convert.ToInt32(val) != Convert.ToInt32(result))
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawColorField(object target, FieldInfo field)
        {
            var label = field.Name;
            var val = (Color)field.GetValue(target);

            var result = EditorGUILayout.ColorField(label, val);
            if (val != result)
            {
                field.SetValue(target, result);
                return true;
            }

            return false;
        }

        private static bool _DrawArrayField(object target, FieldInfo field)
        {
            var isChanged = false;

            var label = field.Name;
            var meta = _FetchMetaInfo(field, label);
            meta.foldout = _EditorGUILayout_Foldout(meta.foldout, label);
            if (meta.foldout)
            {
                EditorGUI.indentLevel++;
                {
                    var filedType = field.FieldType;
                    var elementType = filedType.GetElementType();
                    if (elementType == null)
                    {
                        Console.Error.WriteLine("elementType is null, fieldType={0}", filedType);
                        return false;
                    }

                    // array有可能是null
                    var array = (Array)field.GetValue(target);
                    var length = array?.GetLength(0) ?? 0;

                    var nextLength = EditorGUILayout.DelayedIntField("Size", length);
                    if (nextLength != length)
                    {
                        nextLength = Math.Max(0, nextLength);
                        var nextArray = Array.CreateInstance(elementType, nextLength);

                        var copyLength = Math.Min(nextLength, length);
                        if (null != array && copyLength > 0)
                        {
                            Array.Copy(array, nextArray, copyLength);
                        }

                        field.SetValue(target, nextArray);
                        isChanged = true;

                        array = nextArray;
                        length = nextLength;
                    }

                    if (array != null)
                    {
                        if (elementType.IsValueType)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                var item = array.GetValue(i);
                                var itemName = _FetchElementName(i);
                                var isFieldChanged = DrawObject(item, itemName);
                                if (isFieldChanged)
                                {
                                    isChanged = true;
                                    array.SetValue(item, i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                var item = array.GetValue(i);
                                if (null == item)
                                {
                                    isChanged = true;
                                    item = _CreateInstance(elementType);
                                    array.SetValue(item, i);
                                }

                                var itemName = _FetchElementName(i);
                                isChanged = DrawObject(item, itemName) || isChanged;
                            }
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }

            return isChanged;
        }

        private static object _CreateInstance(Type type)
        {
            // string没有public的ctor()
            if (type == typeof(string))
            {
                return string.Empty;
            }

            return Activator.CreateInstance(type);
        }
        private static bool _DrawListField(object target, FieldInfo field, Type elementType)
        {
            var isChanged = false;

            var label = field.Name;
            var meta = _FetchMetaInfo(field, label);
            meta.foldout = _EditorGUILayout_Foldout(meta.foldout, label);
            if (meta.foldout)
            {
                EditorGUI.indentLevel++;
                {
                    var fieldType = field.FieldType;

                    var list = (IList)field.GetValue(target);
                    var count = null != list ? list.Count : 0;

                    var nextCount = EditorGUILayout.DelayedIntField("Size", count);
                    if (nextCount != count)
                    {
                        nextCount = Math.Max(0, nextCount);

                        var listType = TypeTools.MakeGenericType(typeof(List<>), fieldType.GetGenericArguments());
                        var nextList = _CreateInstance(listType) as IList;

                        var copyCount = Math.Min(nextCount, count);
                        for (int i = 0; i < copyCount; ++i)
                        {
                            nextList.Add(list[i]);
                        }

                        for (int i = copyCount; i < nextCount; ++i)
                        {
                            var item = _CreateInstance(elementType);
                            nextList.Add(item);
                        }

                        field.SetValue(target, nextList);
                        isChanged = true;

                        list = nextList;
                        count = nextCount;
                    }

                    if (elementType.IsValueType)
                    {
                        for (int i = 0; i < nextCount; ++i)
                        {
                            var item = list[i];
                            var itemName = _FetchElementName(i);
                            var isFieldChanged = DrawObject(item, itemName);
                            if (isFieldChanged)
                            {
                                isChanged = true;
                                list[i] = item;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < nextCount; ++i)
                        {
                            var item = list[i];
                            if (null == item)
                            {
                                item = _CreateInstance(elementType);
                                list[i] = item;
                                isChanged = true;
                            }

                            var itemName = _FetchElementName(i);
                            isChanged = DrawObject(item, itemName) || isChanged;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }

            return isChanged;
        }

        private static string _FetchElementName(int index)
        {
            if (index < 0)
            {
                return string.Empty;
            }

            var count = _elementNames.Length;
            if (index >= count)
            {
                var nextCount = Math.Max(count << 1, index + 1);
                var nextElementNames = new string[nextCount];
                Array.Copy(_elementNames, nextElementNames, count);

                _elementNames = nextElementNames;
                for (int i = count; i < nextCount; ++i)
                {
                    _elementNames[i] = "Element " + i;
                }
            }

            return _elementNames[index];
        }

        private static MetaInfo _FetchMetaInfo(object key, string name)
        {
            if (null != key)
            {
                string combinedKey = key.GetType() + "_" + name;
                if (_metaInfos[combinedKey] is not MetaInfo info)
                {
                    info = new MetaInfo();
                    _metaInfos[combinedKey] = info;
                }

                return info;
            }

            return null;
        }

        private static bool _EditorGUILayout_Foldout(bool foldout, string content)
        {
            foldout = EditorGUILayout.Foldout(foldout, content, true);
            return foldout;
        }

        private static string[] _elementNames = Array.Empty<string>();
        private static Hashtable _metaInfos = new();
    }
}