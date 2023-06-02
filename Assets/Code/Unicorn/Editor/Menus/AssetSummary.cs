
/********************************************************************
created:    2014-08-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using System.Text;
using Unicorn.IO;

namespace Unicorn.Menus
{
	internal static class AssetSummary
	{
		[MenuItem("*Metadata/Print Asset Summary", false, 609)]
		private static void _Execute ()
		{
            var asset = Selection.activeObject;
            if (null == asset)
            {
                Logo.Error("Please select one asset.");
                return;
            }

			OpenAssetSummary(asset);
		}

		public static void OpenAssetSummary (UnityEngine.Object asset)
		{
			if (null == asset)
			{
				Logo.Error("[OpenAssetSummary()] asset is null.");
				return;
			}

			var summary = GetAssetSummary(asset);
			FileTools.ShowTempFile(asset.name + ".log", summary);
		}

		public static string GetAssetSummary (UnityEngine.Object asset)
		{
			if (null == asset)
			{
				return string.Empty;
			}

			var assetName = asset.name;
			var sbText = new StringBuilder(1024);
			sbText.AppendFormat ("name={0}\n", assetName);

			var assetPath = AssetTools.GetAssetPath(asset);
			sbText.AppendFormat("assetPath={0}\n", assetPath);

			var md5 = FileTools.GetHexDigest16(assetPath);
			sbText.AppendFormat("md5={0}\n\n", md5);

			var roots = new[] { asset };
			var dependencies = EditorUtility.CollectDependencies(roots);
			var count = dependencies.Length;
			sbText.AppendFormat("asset dependencies ({0}):\n", count.ToString());
			for (int i= 0; i< count; ++i)
			{
				var partAsset = dependencies[i];
				var partAssetPath = AssetTools.GetAssetPath(partAsset);
				sbText.AppendFormat("\t{0, -30}\t{1, -15}\t{2}\n"
				                    , partAsset.name
				                    , partAsset.GetType().Name
				                    , partAssetPath);
			}

			var summary = sbText.ToString();
			return summary;
		}
    }
}
