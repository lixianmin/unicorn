/********************************************************************
created:    2015-03-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using System;
using System.Reflection;

namespace Unicorn.Reflection
{
//	[Flags]
//	public enum ImportAssetOptions
//	{
//		Default = 0,
//		ForceUpdate = 1,
//		ForceSynchronousImport = 8,
//		ImportRecursive = 256,
//		DontDownloadFromCacheServer = 8192,
//		ForceUncompressedImport = 16384
//	}

    public static class AssetDatabase
    {
        private static Action _lpfnSaveAssets;

        public static void SaveAssets()
        {
            if (null == _lpfnSaveAssets)
            {
                var method = MyType.GetMethod("SaveAssets",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                TypeTools.CreateDelegate(method, out _lpfnSaveAssets);
            }

            _lpfnSaveAssets();
        }

        private static Func<UnityEngine.Object, string> _lpfnGetAssetPath;

        public static string GetAssetPath(UnityEngine.Object assetObject)
        {
            if (null == _lpfnGetAssetPath)
            {
                const BindingFlags flags = System.Reflection.BindingFlags.Public |
                                           System.Reflection.BindingFlags.Static;
                var method = MyType.GetMethod("GetAssetPath", flags, null, new[] { typeof(UnityEngine.Object) }, null);
                TypeTools.CreateDelegate(method, out _lpfnGetAssetPath);
            }

            return _lpfnGetAssetPath(assetObject);
        }

        private static Func<string, string[], string[]> _lpfnFindAssets;

        public static string[] FindAssets(string filter, string[] searchInFolders)
        {
            if (null == _lpfnFindAssets)
            {
                var method = _GetPublicStaticMethod("FindAssets", new[] { typeof(string), typeof(string[]) });
                TypeTools.CreateDelegate(method, out _lpfnFindAssets);
            }

            return _lpfnFindAssets(filter, searchInFolders);
        }

        private static Func<string, string> _lpfnGUIDToAssetPath;

        public static string GUIDToAssetPath(string guid)
        {
            if (null == _lpfnGUIDToAssetPath)
            {
                var method = _GetPublicStaticMethod("GUIDToAssetPath", new[] { typeof(string) });
                TypeTools.CreateDelegate(method, out _lpfnGUIDToAssetPath);
            }

            return _lpfnGUIDToAssetPath(guid);
        }

        private static Func<string, string> _lpfnAssetPathToGUID;

        public static string AssetPathToGUID(string path)
        {
            if (null == _lpfnAssetPathToGUID)
            {
                var method = _GetPublicStaticMethod("AssetPathToGUID", new[] { typeof(string) });
                TypeTools.CreateDelegate(method, out _lpfnAssetPathToGUID);
            }

            return _lpfnAssetPathToGUID(path);
        }

        private static Func<string, UnityEngine.Object> _lpfnLoadMainAssetAtPath;

        public static UnityEngine.Object LoadMainAssetAtPath(string assetPath)
        {
            if (null == _lpfnLoadMainAssetAtPath)
            {
                var method = _GetPublicStaticMethod("LoadMainAssetAtPath", new[] { typeof(string) });
                TypeTools.CreateDelegate(method, out _lpfnLoadMainAssetAtPath);
            }

            return _lpfnLoadMainAssetAtPath(assetPath);
        }

        private static MethodInfo _GetPublicStaticMethod(string methodName, Type[] types)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            var method = MyType.GetMethod(methodName, flags, null, types, null);
            return method;
        }

        private static Type _myType;

        public static Type MyType
        {
            get
            {
                if (null == _myType)
                {
                    _myType = System.Type.GetType("UnityEditor.AssetDatabase,UnityEditor");
                }

                return _myType;
            }
        }
    }
}