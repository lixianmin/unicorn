
/********************************************************************
created:    2014-12-23
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/
#pragma warning disable 0618 //Warning CS0618: `UnityEngine.Object.FindObjectsOfTypeIncludingAssets(System.Type)' is obsolete: `use Resources.FindObjectsOfTypeAll instead.'

using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Object = UnityEngine.Object;

namespace Unicorn
{
    public static class AssetTools
    {
		static AssetTools ()
		{
			_svnFolder = os.isWindows ? "\\.svn\\" : "/.svn/";
		}

        internal static void ApplyPrefab (GameObject instance)
        {
            PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
        }

		/// <summary>
		/// scene root game objects.
		/// </summary>
		/// <returns>The scene game objects.</returns>
        public static GameObject[] CollectSceneGameObjects ()
        {
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			var roots = scene.GetRootGameObjects();
			return roots;
        }

		// Do not Resources.FindObjectsOfTypeAll(), which will return a huge amount of resources.
		internal static T[] FindObjectsOfTypeIncludingAssets<T> () where T: Object
		{
			var objs = (T[]) Object.FindObjectsOfTypeIncludingAssets(typeof(T));
			return objs;
		}

        internal static bool IsAtlas (Object asset)
        {
            var go = asset as GameObject;
            return null != go && ( null != go.GetComponent("UIAtlas") || null != go.GetComponent("UI2DSpriteAltasEx") );
        }

		// for some reasons, AssetDatabase.LoadAssetAtPath() will return an asset with m_InstanceID=0,
		// which is a not-null asset but can not call any methods on it except GetInstanceID().
		private static bool _IsValidAsset (Object asset)
		{
			return null != asset && asset.GetInstanceID() != 0;
		}

		internal static UnityEngine.Object LoadAssetAtPath (string assetPath)
		{
			return LoadAssetAtPath<UnityEngine.Object>(assetPath);
		}

		internal static T LoadAssetAtPath<T> (string assetPath) where T : UnityEngine.Object
		{
			var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
			if (_IsValidAsset(asset))
			{
				return asset;
			}

			return null;
		}

		public static IEnumerable<string> EnumerateSelectedAssetPaths ()
		{
			foreach (var guid in Selection.assetGUIDs)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath (guid);
				if (Directory.Exists(assetPath))
				{
					foreach (var filepath in os.walk(assetPath, "*.*"))
					{
						if (_IsValidAssetPath(assetPath))
						{
							yield return filepath;
						}
					}
				}
				else if (_IsValidAssetPath(assetPath))
				{
					yield return assetPath;
				}
			}
		}

		internal static string[] GetAssetPaths (string directoryPath)
		{
			if (null != directoryPath && System.IO.Directory.Exists(directoryPath))
			{
				var filePaths = os.walk(directoryPath, "*.*");

				foreach (var assetPath in filePaths)
				{
					if (_IsValidAssetPath(assetPath))
					{
						var normlizedAssetPath = os.path.normpath(assetPath);
						_asssetPaths.Add(normlizedAssetPath);
					}
				}
			}

			var assetPaths = Array.Empty<string>();
			if (_asssetPaths.Count > 0)
			{
				_asssetPaths.Sort((a, b) => a.CompareTo(b));
				assetPaths = _asssetPaths.ToArray();
				_asssetPaths.Clear();
			}
			
			return assetPaths;
		}

		private static bool _IsValidAssetPath (string assetPath)
		{
			if (assetPath.EndsWithEx(_notAssetExtensions, CompareOptions.Ordinal))
			{
				return false;
			}
			
			if (assetPath.Contains(_svnFolder))
			{
				return false;
			}
            if (os.path.normpath(assetPath).Contains(".svn/"))
            {
                return false;
            }

			return true;
		}

		internal static UnityEngine.Object[] LoadAssets (string[] assetPaths)
		{
			if (null == assetPaths)
			{
				return null;
			}

			var count = assetPaths.Length;
			if (count == 0)
			{
				return null;
			}

			var assets = new UnityEngine.Object[count];
			for (int i= 0; i< count; ++i)
			{
				assets[i] = AssetTools.LoadAssetAtPath(assetPaths[i]);
			}

			return assets;
		}

		internal static void SetDirty (UnityEngine.Object target)
		{
			if (null != target)
			{
				EditorUtility.SetDirty(target);
				_hasDirtyObjects = true;
			}
		}

		internal static string GetAssetPath (UnityEngine.Object asset)
		{
			if (!asset)
			{
				return string.Empty;
			}

			var path = AssetDatabase.GetAssetPath(asset);
			if (string.IsNullOrEmpty(path))
			{
				Logo.Error("[GetAssetPath()] GetAssetPath() returns null, asset={0}", asset);
				return string.Empty;
			}

			return path;
		}

		internal static void SaveAssets ()
		{
			if (_hasDirtyObjects)
			{
				AssetDatabase.SaveAssets();
				_hasDirtyObjects = false;
			}
		}

		internal static Object[] CollectDependencies (Object asset)
		{
			if (null != asset)
			{
				_collectDependenciesRoots[0] = asset;
				var result = EditorUtility.CollectDependencies(_collectDependenciesRoots);
				return result;
			}

			return Array.Empty<Object>();
		}

		private static bool _hasDirtyObjects;

		private static readonly List<string> _asssetPaths = new();

		private static readonly string _svnFolder;

		private static readonly string[] _notAssetExtensions = {
			".meta",
			".DS_Store",
		};

		private static readonly Object[] _collectDependenciesRoots = new Object[1];
    }
}