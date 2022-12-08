
/********************************************************************
created:    2014-09-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using UnityEditor;

namespace Unicorn.Menus
{
    internal class OpenInformation
    {
        [MenuItem("*Metadata/Open Information ^i", false, 600)]
        private static void _Execute ()
        {
            new OpenInformation()._Run();
        }

        private void _Run ()
        {
			var platform = Application.platform;
			if (platform == RuntimePlatform.OSXEditor)
			{
				_OSX_Select ();
			}
			else
			{
				Console.Warning.WriteLine("To be implemented.");
			}
        }

//		private void _OnSelectSceneGameObject (GameObject go)
//		{
//			var labels	= go.GetComponentsInChildren(NGUITypes.UILabel, true);
//			var sprites = go.GetComponentsInChildren(NGUITypes.UISprite, true);
//			var buttons = go.GetComponentsInChildren(NGUITypes.UIButton, true);
//
//			Console.Warning.WriteLine("name={0} labels={1} sprites={2} buttons={3}"
//			                          , go.name
//			                          , labels.Length.ToString()
//			                          , sprites.Length.ToString()
//			                          , buttons.Length.ToString());
//		}

		private static void _OSX_Select ()
		{
			foreach (var guid in Selection.assetGUIDs)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath (guid);
				_OSX_OnSelectAsset(assetPath);
			}
		}

		private static void _OSX_OnSelectAsset (string assetPath)
		{
			var fullpath  = os.path.join(PathTools.ProjectPath, assetPath);
			
			var scriptCode = string.Format("-e 'tell application \"Finder\" '" +
			                               "\n-e 'open information window of (POSIX file \"{0}\" as alias)'" +
			                               "\n-e 'activate'" +
			                               "\n-e 'end tell'", fullpath);
			
			os.startfile("osascript", scriptCode, true);
		}
    }
}