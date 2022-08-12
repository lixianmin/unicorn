﻿
/********************************************************************
created:    2014-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using System.IO;
using System.Globalization;
using Unicorn;

namespace Metadata.Menus
{
    class ClearAutoCode
    {
        [MenuItem(EditorMetaTools.MenuRoot + "Clear Auto Code", true)]
		private static bool _Clear_Validate ()
		{
			var manifest = UnicornManifest.OpenOrCreate();
			return manifest.ClearAutoCode;
		}

        [MenuItem(EditorMetaTools.MenuRoot + "Clear Auto Code", false, 103)]
        public static void Clear ()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Warning", "Wait for compiling", "OK");
                return;
            }

            var directories = new string[]
            {
                EditorMetaTools.StandardAutoCodeDirectory,
                EditorMetaTools.ClientAutoCodeDirectory,
//				EditorMetaCommon.EditorAutoCodeDirectory,
//                EditorMetaCommon.DrawableObjectsDirectory,
            };

            for (int i= 0; i< directories.Length; ++i)
            {
                var directory = directories[i];
                if(!Directory.Exists(directory))
                {
                    continue;
                }

                var files = Directory.GetFiles(directories[i]);
                for (int j= 0; j< files.Length; ++j)
                {
                    var filename = files[j];
                    if (!filename.EndsWithEx(".meta", CompareOptions.Ordinal))
                    {
                        using var fout = File.Create(filename);
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}