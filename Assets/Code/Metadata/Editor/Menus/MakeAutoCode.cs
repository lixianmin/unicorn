
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
			var isValid = manifest.makeAutoCode && !EditorApplication.isCompiling;
			return isValid;
		}

        [MenuItem(EditorMetaTools.MenuRoot + "Make Auto Code", false, 102)]
		public static void Make ()
        {
            // if (EditorApplication.isCompiling)
            // {
            //     EditorUtility.DisplayDialog("Warning", "Wait for compiling", "OK");
            //     return;
            // }

            // make auto code之前先Refresh()一把, 因为通常只所以需要make auto code就是因为加入了新的类型, 这时如果不Refresh()一把就找不到新的类型
            AssetDatabase.Refresh();
            
            new MetaFactoryCodeMaker().WriteAll();
			_RemoveEmptyFiles(EditorMetaTools.StandardAutoCodeDirectory);
            _RemoveEmptyFiles(EditorMetaTools.ClientAutoCodeDirectory);
            
            // make auto code之后, 但因为执行时会自动Refresh()，所以这里似乎不需要Refresh
			// AssetDatabase.Refresh();

            // lua的脚本暂时先用不到了
			// new LuaCodeMaker().WriteAll();
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