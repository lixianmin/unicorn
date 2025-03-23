/********************************************************************
created:    2017-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unicorn;
using Unicorn.IO;
using Unicorn.Collections;

namespace Metadata
{
    partial class MetaTools
    {
        static class ListUtil
        {
            class ListBuilder
            {
                private ListBuilder()
                {
                }

                public static ListBuilder Create(Type fieldType)
                {
                    if (null == fieldType)
                    {
                        return null;
                    }

                    if (_builderCache[fieldType] is not ListBuilder cachedBuilder)
                    {
                        var argumentTypes = fieldType.GetGenericArguments();
                        var baseListType = typeof(List<>);
                        var listType = TypeTools.MakeGenericType(baseListType, argumentTypes);

                        cachedBuilder = new ListBuilder
                        {
                            _constructor = _GetListConstructor(listType),
                            _elementType = argumentTypes[0]
                        };

                        _builderCache[fieldType] = cachedBuilder;
                    }

                    return cachedBuilder;
                }

                private static ConstructorInfo _GetListConstructor(Type listType)
                {
                    var flags = BindingFlags.Instance | BindingFlags.Public;
                    var constructors = listType.GetConstructors(flags);
                    var count = constructors.Length;

                    for (int i = count - 1; i >= 0; i--)
                    {
                        var constructor = constructors[i];
                        var args = constructor.GetParameters();

                        if (args.Length == 1 && args[0].ParameterType == typeof(int))
                        {
                            return constructor;
                        }
                    }

                    return null;
                }

                public IList CreateList(int count)
                {
                    var invokeParams = new object[] { count };
                    var list = (IList)_constructor.Invoke(BindingFlags.CreateInstance, null, invokeParams, null);
                    // IList list1 = (IList) Activator.CreateInstance(listType, _listInvokeParams, EmptyArray<object>.Instance);

                    return list;
                }

                public Type GetElementType()
                {
                    return _elementType;
                }

                private ConstructorInfo _constructor;
                private Type _elementType;

                private static readonly Dictionary<Type, object> _builderCache = new();
            }

            public static IList Load(IOctetsReader reader, Type fieldType)
            {
                var basicType = (BasicType)reader.ReadByte();
                int count = reader.ReadInt32();

                // Type elementType = fieldType.GetGenericArguments()[0];
                // IList list = (IList)Activator.CreateInstance(fieldType);

                var builder = ListBuilder.Create(fieldType);
                var elementType = builder.GetElementType();
                var list = builder.CreateList(count);

                for (int i = 0; i < count; ++i)
                {
                    var val = _LoadObject(reader, elementType, basicType);
                    list.Add(val);
                }

                return list;
            }

            public static void Save(IOctetsWriter writer, object target, Type targetType, bool isFullMode)
            {
                var list = target as IList;

                Type elementType = list?.GetElementType();
                var basicType = null != elementType ? GetBasicType(elementType) : BasicType.Null;
                writer.Write((byte)basicType);

                var count = list?.Count ?? 0;
                writer.Write(count);

                for (int i = 0; i < count; ++i)
                {
                    var item = list![i];
                    _SaveObject(writer, item, elementType, isFullMode);
                }
            }

            public static bool IsEqual(object lhsTarget, object rhsTarget)
            {
                var lhsList = lhsTarget as IList;
                var rhsList = rhsTarget as IList;

                var lhsCount = lhsList?.Count ?? 0;
                var rhsCount = rhsList?.Count ?? 0;
                if (lhsCount != rhsCount)
                {
                    return false;
                }

                if (lhsCount == 0)
                {
                    return true;
                }

                Type lhsElementType = lhsList.GetElementType();
                Type rhsElementType = rhsList.GetElementType();
                if (lhsElementType != rhsElementType)
                {
                    return false;
                }

                for (int i = 0; i < lhsCount; ++i)
                {
                    var lhsItem = lhsList[i];
                    var rhsItem = rhsList[i];

                    if (!_IsEqual(lhsItem, rhsItem, lhsElementType))
                    {
                        return false;
                    }
                }

                return true;
            }

//            public static IEnumerable EnumerateFields (object target, Type targetType)
//            {
//                var list = target as IList;
//                Type elementType = null != list ? list.GetElementTypeEx() : null;
//                var count = null != list ? list.Count : 0;
//
//                for (int i= 0; i< count; ++i)
//                {
//                    var item = list[i];
//                    foreach (var ret in _EnumerateObject(item, elementType))
//                    {
//                        yield return ret;
//                    }
//                }
//            }
        }
    }
}