
/********************************************************************
created:    2014-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using Unicorn;
using System.IO;
using System.Globalization;
using Metadata.LuaCode;

namespace Metadata.Menus
{
    public static class MakeAutoCode
    {
        [MenuItem(EditorMetaTools.MenuRoot + "Make Auto Code", true)]
		private static bool _Make_Validate ()
		{
			var manifest = UnicornManifest.OpenOrCreate();
			return manifest.MakeAutoCode;
		}

        [MenuItem(EditorMetaTools.MenuRoot + "Make Auto Code", false, 102)]
		public static void Make ()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Warning", "Wait for compiling", "OK");
                return;
            }

            new MetaFactoryCodeMaker().WriteAll();
			_RemoveEmptyFiles(EditorMetaTools.StandardAutoCodeDirectory);
            _RemoveEmptyFiles(EditorMetaTools.ClientAutoCodeDirectory);
			AssetDatabase.Refresh();

			new LuaCodeMaker().WriteAll();
        }

        private static void _RemoveEmptyFiles (string directory)
        {
			if (!Directory.Exists(directory))
			{
				return;
			}

            foreach (var filename in Directory.GetFiles(directory))
            {
                if (filename.EndsWithEx(".cs", CompareOptions.Ordinal))
                {
                    var info = new FileInfo(filename);
                    if(info.Length == 0)
                    {
                        File.Delete(filename);
                    }
                }
            }
        }
    }
}