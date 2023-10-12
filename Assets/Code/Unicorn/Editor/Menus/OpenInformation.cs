/********************************************************************
created:    2014-09-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Unicorn.Menus
{
    internal class OpenInformation
    {
        [MenuItem("*Tools/Open Information ^i", false, 600)]
        private static void _Execute()
        {
            _Run();
        }

        private static void _Run()
        {
            var platform = Application.platform;
            if (platform == RuntimePlatform.OSXEditor)
            {
                _OSX_Select();
            }
            else
            {
                Logo.Warn("To be implemented.");
            }
        }

//		private void _OnSelectSceneGameObject (GameObject go)
//		{
//			var labels	= go.GetComponentsInChildren(NGUITypes.UILabel, true);
//			var sprites = go.GetComponentsInChildren(NGUITypes.UISprite, true);
//			var buttons = go.GetComponentsInChildren(NGUITypes.UIButton, true);
//
//			Logo.Warn("name={0} labels={1} sprites={2} buttons={3}"
//			                          , go.name
//			                          , labels.Length.ToString()
//			                          , sprites.Length.ToString()
//			                          , buttons.Length.ToString());
//		}

        private static void _OSX_Select()
        {
            foreach (var guid in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                _OSX_OnSelectAsset(assetPath);
            }
        }

        private static void _OSX_OnSelectAsset(string assetPath)
        {
            var fullPath = os.path.join(PathTools.ProjectPath, assetPath);
            var scriptPath = os.path.join(Path.GetTempPath(), "info.script");
            // Logo.Info(scriptPath);

            var scriptCode = "tell application \"Finder\"\n" +
                             $"open information window of (POSIX file \"{fullPath}\" as alias)\n" +
                             "activate\n" +
                             $"delete POSIX file \"{scriptPath}\"\n" +
                             "end tell";
            File.WriteAllText(scriptPath, scriptCode);
            Kernel.StartFile("osascript", scriptPath, true);
        }
    }
}