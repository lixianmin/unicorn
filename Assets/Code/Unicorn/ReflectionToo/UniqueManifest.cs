﻿
/********************************************************************
created:    2015-03-30
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using UnityEngine;
using Unicorn.Reflection;

namespace Unicorn
{
	[Serializable]
    public class UnicornManifest
    {
		public enum PathType
		{
			EditorResourceRoot,	// for exporting and loading runtime resources.
			LuaScriptRoot,		// lua code may reside in git, company with c# code.
			MetadataRoot,		// metadata root directory.
			UserdataBase =  5,	// used by other editors.
		}

		private UnicornManifest ()
		{

		}

		public static UnicornManifest OpenOrCreate ()
		{
			var manifestPath = _GetManifestPath();

			if (File.Exists(manifestPath))
			{
				var manifest = XmlTools.Deserialize<UnicornManifest>(manifestPath);
				return manifest;
            }
			else
			{
				var manifest = new UnicornManifest();
				XmlTools.Serialize(manifestPath, manifest);
                
                return manifest;
            }
		}

		private static string _GetManifestPath ()
		{
			var path = os.path.join(Application.dataPath, "unicorn_manifest.xml");
			return path;
		}

		private string _CheckChoosePath (PathType pathType, string title, bool manualSelect)
		{
			var index = (int) pathType;
			if (null == RelativePaths || index >= RelativePaths.Length)
			{
				var minCount = index + 1;
				Array.Resize(ref RelativePaths, minCount);
			}

			if (string.IsNullOrEmpty(RelativePaths[index]))
			{
				if (!manualSelect)
				{
					return string.Empty;
				}

				var dataPath = Application.dataPath;
				var folderPath = EditorUtility.OpenFolderPanel(title, dataPath, string.Empty);
				
				var uri1 = new Uri(dataPath);
				var uri2 = new Uri(folderPath);
				var relativePath = uri1.MakeRelativeUri(uri2).ToString();
				
				if (Directory.Exists(relativePath))
				{
					RelativePaths[index] = relativePath;

					var manifestPath = _GetManifestPath();
					XmlTools.Serialize(manifestPath, this);
				}
			}
			
			var fullpath = Path.GetFullPath(RelativePaths[index]);
			return fullpath;
		}

		internal static string _GetEditorResourceRoot ()
		{
			var title = "Please choose the editor resource root for exporting or downloading assetBundles";
			var manifest = OpenOrCreate();
			var fullpath = manifest._CheckChoosePath(PathType.EditorResourceRoot, title, true);
			return fullpath;
		}

		public static string GetLuaScriptRoot ()
		{
            if (!Application.isEditor)
            {
                return "";
            }
			var title = "Please choose lua script root directory";
			var manifest = OpenOrCreate();
			var fullpath = manifest._CheckChoosePath(PathType.LuaScriptRoot, title, true);
			return fullpath;
		}

		public static string GetMetadataRoot ()
		{
			var title = "Please choose metadata root directory";
			var manifest = OpenOrCreate();
			var fullpath = manifest._CheckChoosePath(PathType.MetadataRoot, title, true);
			return fullpath;
		}

		public static string GetUserdataRoot (PathType pathType, string title)
		{
			var manifest = OpenOrCreate();
			var fullpath = manifest._CheckChoosePath(pathType, title, true);
			return fullpath;
		}
		
		public string[] RelativePaths;

		public bool MakeAutoCode;
		public bool ClearAutoCode;
		public string spriteDirPath = "Assets/Art/UIResource/UI/Sprite"; // 存放碎图sprite的文件夹地址.
        public string uiPrefabDirPath = "Assets/prefabs/ui"; // ui界面prefab的文件夹地址.
    }
}