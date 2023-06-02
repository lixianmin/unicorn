
/********************************************************************
created:    2018-03-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using Unicorn;
using UnityEngine;

namespace Metadata
{
    public static class EditorMetaTools
    {
        static EditorMetaTools ()
        {
            _metaTypes = _CollectAllMetaTypes();
        }

        private static MetaType[] _CollectAllMetaTypes ()
        {
            var list = new List<MetaType>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try 
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (MetaTools.IsMetadata(type))
                        {
                            var metaType = new MetaType(type);
                            list.Add(metaType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logo.Error("assembly={0}, ex={1}", assembly.FullName, ex);
                }
            }

            list.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            var types = list.ToArray();

            return types;
        }

        public static IEnumerable<MetaType> EnumerateTemplateTypes ()
        {
            foreach (var type in GetMetaTypes())
            {
                if (type.IsTemplate)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<MetaType> EnumerateConfigTypes ()
        {
            foreach (var type in GetMetaTypes())
            {
                if (type.IsConfig)
                {
                    yield return type;
                }
            }
        }

        internal static IEnumerable<MetaType> EnumerateLuaTypes ()
        {
            foreach (var type in GetMetaTypes())
            {
                var flags = type.GetExportFlags();
                if ((flags & ExportFlags.ExportLua) != 0)
                {
                    yield return type;
                }
            }
        }

        public static MetaType[] GetMetaTypes ()
        {
            return _metaTypes;
        }

        internal static ExportFlags GetExportFlags (Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(ExportAttribute), false);

            foreach (ExportAttribute attribute in attributes)
            {
                var flags = attribute.GetExportFlags();
                return flags;
            }

            return ExportFlags.ExportRaw;
        }

        internal static bool IsAutoCodeIgnore (MemberInfo member)
        {
            var ignore = member.GetCustomAttributes(typeof(AutoCodeIgnoreAttribute), false).Length > 0;
            return ignore;
        }

        private static string _standardAutoCodeDirectory;
        internal static string StandardAutoCodeDirectory
        {
            get
            {
                if (null == _standardAutoCodeDirectory)
                {
                    _standardAutoCodeDirectory = Application.dataPath + "/Standard Assets/Code/Metadata/AutoCode/";
                }

                return _standardAutoCodeDirectory;
            }
        }

        private static string _clientAutoCodeDirectory;
        internal static string ClientAutoCodeDirectory
        {
            get
            {
                if (null == _clientAutoCodeDirectory)
                {
                    _clientAutoCodeDirectory = Application.dataPath + "/Code/Metadata/AutoCode/";
                }

                return _clientAutoCodeDirectory;
            }
        }

        public const string MenuRoot = "*Metadata/";

        private static MetaType[] _metaTypes;
    }
}