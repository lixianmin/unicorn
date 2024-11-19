/********************************************************************
created:    2014-03-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.IO;
using Unicorn.Security;

namespace Metadata
{
    public static class MetaFactory
    {
        internal static MetaCreator GetMetaCreator(string typeName)
        {
            var lookupTable = _GetLookupTableByName();
            var creator = lookupTable[typeName] as MetaCreator;
            if (creator == null)
            {
                Logo.Error("creator=null, typeName={0}, lookupTable.Count={1}", typeName, lookupTable.Count);
            }

            return creator;
        }

        public static XTEACrypto CreateCrypto()
        {
            var round = 8;
            var key = new uint[] { 0xA1, 0xDD, 0xB5, 0x56 };
            var crypto = new XTEACrypto(round, key);
            return crypto;
        }

        internal static OctetsReader CreateChunkReader(Stream stream)
        {
            if (null == stream || !stream.CanRead)
            {
                return null;
            }

            var mode = OctetsMode.UseFilter | OctetsMode.Compress;
            var reader = new OctetsReader(stream, mode);

            return reader;
        }

        internal static OctetsReader CreateChunkReader(MetadataManager manager, bool isFullMode)
        {
            if (null == manager)
            {
                return null;
            }

            var stream = new MemoryStream(8192);
            var aid = new SaveAid(null);
            aid.Save(stream, manager, isFullMode);

            stream.Position = 0;
            var reader = CreateChunkReader(stream);
            return reader;
        }

        internal static IEnumerable<MetaCreator> EnumerateMetaCreators()
        {
            var lookupTableByName = _GetLookupTableByName();
            var it = lookupTableByName.GetEnumerator();
            while (it.MoveNext())
            {
                var creator = it.Value as MetaCreator;
                yield return creator;
            }
        }

        private static Hashtable _GetLookupTableByName()
        {
            if (null == _lookupTableByName)
            {
                var outerFactoryTypeName = "Metadata." + outerFactoryName;
                var outerFactoryType = TypeTools.SearchType(outerFactoryTypeName);

                if (null != outerFactoryType)
                {
                    var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
                    var method = outerFactoryType.GetMethod(outerFactoryGetLookupTableByName, bindingFlags);

                    TypeTools.CreateDelegate(method, out Func<Hashtable> functor);

                    if (null != functor)
                    {
                        _lookupTableByName = functor();
                    }
                }

                if (null == _lookupTableByName)
                {
                    Logo.Error(
                        "[_GetLookupTableByName()] _lookupTableByName is null, outerFactoryTypeName={0}, outerFactoryGetLookupTableByType={1}"
                        , outerFactoryTypeName, outerFactoryGetLookupTableByName);

                    _lookupTableByName = new Hashtable();
                }
                else
                {
                    _CollectEditorOnlyMetadataTypes(_lookupTableByName);
                }
            }

            return _lookupTableByName;
        }

        // 特定于lua的metadata定义写在Assembly-CSharp-Editor.dll中，这些在自动生成代码时
        // 可以不生成到_MetaFactory.cs中
        private static void _CollectEditorOnlyMetadataTypes(Hashtable lookupTableByName)
        {
            var assembly = TypeTools.GetEditorAssembly();
            if (null == assembly)
            {
                return;
            }

            const bool isEditorOnlyCreator = true;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (MetaTools.IsMetadata(type))
                {
                    var key = type.FullName!;
                    var info = type.GetConstructor(Type.EmptyTypes);
                    var val = new MetaCreator(() => (IMetadata)info!.Invoke(null), isEditorOnlyCreator);
                    lookupTableByName.Add(key, val);
                }
            }
        }

        private static List<Type> _FetchMetaTypes()
        {
            var list = new List<Type>();
            var lookupTable = _GetLookupTableByName();
            foreach (MetaCreator creator in lookupTable.Values)
            {
                var item = creator.Create();
                var type = item?.GetType();
                list.Add(type);
            }

            return list;
        }

        internal static IList<Type> GetSubTypeList(Type type)
        {
            if (type != null)
            {
                if (_subTypes == null)
                {
                    _subTypes = new();
                    var metaTypes = _FetchMetaTypes();
                    foreach (var item in metaTypes)
                    {
                        var t = item;
                        while (true)
                        {
                            var baseType = t.BaseType!;
                            var isTopType = baseType == typeof(Template) || baseType == typeof(Config) || baseType == typeof(object);
                            if (isTopType)
                            {
                                break;
                            }

                            if (!_subTypes.TryGetValue(baseType, out var list))
                            {
                                list = new List<Type>();
                                _subTypes.Add(baseType, list);
                            }

                            list.Add(t);
                            t = baseType;
                        }
                    }
                }

                if (_subTypes.TryGetValue(type, out var result))
                {
                    return result;
                }
            }

            return Array.Empty<Type>();
        }

        public static void Clear()
        {
            _lookupTableByName = null;
            _subTypes = null;
        }

        public static readonly string outerFactoryName = "OuterMetaFactory";
        public static readonly string outerFactoryGetLookupTableByName = "_GetLookupTableByName";

        private static Hashtable _lookupTableByName;
        private static Dictionary<Type, List<Type>> _subTypes = null;
    }
}