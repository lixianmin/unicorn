
/********************************************************************
created:    2022-08-11
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using Unicorn.Collections;

using Assembly = System.Reflection.Assembly;
using BindingFlags = System.Reflection.BindingFlags;
using FieldInfo = System.Reflection.FieldInfo;

namespace Unicorn
{
    public static partial class TypeTools
    {
        private class AssemblyComparer : IComparer<Assembly>
        {
            public int Compare(Assembly lhs, Assembly rhs)
            {
                var lhsFullName = lhs.FullName;
                var rhsFullName = rhs.FullName;

                foreach (var header in _nameHeaders)
                {
                    var lhsStartOk = lhsFullName.StartsWith(header, CompareOptions.Ordinal);
                    var rhsStartOk = rhsFullName.StartsWith(header, CompareOptions.Ordinal);

                    if (lhsStartOk && !rhsStartOk)
                    {
                        return -1;
                    }

                    if (!lhsStartOk && rhsStartOk)
                    {
                        return 1;
                    }
                }

                // version = "0.0.0.0" means this is an our own assembly.
                var leftVersion = lhsFullName.Split(_splitter)[1];
                var rightVersion = rhsFullName.Split(_splitter)[1];

                var result = leftVersion.CompareTo(rightVersion);
                if (result == 0)
                {
                    result = lhsFullName.CompareTo(rhsFullName);
                }

                return result;
            }

            private readonly char[] _splitter = new char[] { '=' };
            private readonly string[] _nameHeaders = new string[] { "Unicorn", "Assembly-CSharp", "UnityEngine" };
        }

        static TypeTools()
        {
            Clear();
            _Init_MakeGenericType();
        }

        public static Type SearchType(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                return null;
            }

            if (null == _currentAssemblies)
            {
                _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                Array.Sort(_currentAssemblies, new AssemblyComparer());
            }

            var count = _currentAssemblies.Length;
            for (int i = 0; i < count; ++i)
            {
                var assembly = _currentAssemblies[i];
                var type = assembly.GetType(typeFullName);

                if (null != type)
                {
                    return type;
                }
            }

            return null;
        }

        public static FieldInfo[] GetSortedFields(Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (null != type)
            {
                if (_sortedFields[type] is not FieldInfo[] fields)
                {
                    fields = type.GetFields(flags);
                    Array.Sort(fields, (x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
                    _sortedFields[type] = fields;
                }

                return fields;
            }

            return Array.Empty<FieldInfo>();
        }

        public static void CreateDelegate<T>(System.Reflection.MethodInfo method, out T lpfnMethod) where T : class
        {
            lpfnMethod = Delegate.CreateDelegate(typeof(T), method, false) as T;
        }

        public static void CreateDelegate<T>(object target, string methodName, out T lpfnMethod) where T : class
        {
            lpfnMethod = Delegate.CreateDelegate(typeof(T), target, methodName, false, false) as T;
        }

        public static void CreateDelegate<T>(Type targetType, string methodName, out T lpfnMethod) where T : class
        {
            lpfnMethod = Delegate.CreateDelegate(typeof(T), targetType, methodName, false, false) as T;
        }

        public static System.Reflection.Assembly[] GetCustomAssemblies()
        {
            if (null == _customAssemblies)
            {
                var results = new List<System.Reflection.Assembly>();
                var validPrefixes = new[]
                    {
                        "Unicorn,",
                        "Unicorn.Core,",
                        "Unicorn.Editor,",

                        "Assembly-CSharp-firstpass,",
                        "Assembly-CSharp,",
                        "Assembly-CSharp-Editor,",
                    };

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var fullname = assembly.FullName;
                    if (fullname.StartsWith(validPrefixes, CompareOptions.Ordinal))
                    {
                        results.Add(assembly);
                    }
                }

                _customAssemblies = results.ToArray();
            }

            return _customAssemblies;
        }

        public static System.Reflection.Assembly GetEditorAssembly()
        {
            if (null == _editorAssembly)
            {
                foreach (var assembly in GetCustomAssemblies())
                {
                    var fullname = assembly.FullName;
                    if (fullname.StartsWith("Assembly-CSharp-Editor,", CompareOptions.Ordinal))
                    {
                        _editorAssembly = assembly;
                        break;
                    }
                }
            }

            return _editorAssembly;
        }

        public static Type MakeGenericType(Type genericType, Type[] types)
        {
            if (null == genericType || null == types)
            {
                return null;
            }

            return _lpfnMakeGenericType(genericType, types);
        }

        private static void _Init_MakeGenericType()
        {
            _lpfnMakeGenericType = null;

            var typeName = "System.Type,mscorlib";
            var methodName = "MakeGenericType";

            var type = System.Type.GetType(typeName);
            TypeTools.CreateDelegate(type, methodName, out _lpfnMakeGenericType);

            if (null == _lpfnMakeGenericType)
            {
                _lpfnMakeGenericType = (genericType, types) => genericType.MakeGenericType(types);
            }
        }

        public static void Clear()
        {
            _sortedFields.Clear();

            _editorAssembly = null;
            _currentAssemblies = null;
            _customAssemblies = null;
        }

        private static readonly WeakTable _sortedFields = new(1000);
        private static Func<Type, Type[], Type> _lpfnMakeGenericType;

        private static System.Reflection.Assembly _editorAssembly;
        private static System.Reflection.Assembly[] _currentAssemblies;
        private static System.Reflection.Assembly[] _customAssemblies;
    }
}