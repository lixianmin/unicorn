
/********************************************************************
created:    2022-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using System.Linq;
using Unicorn.IO;
using UnityEditor;

namespace Unicorn
{
    [InitializeOnLoad]
    public class ReimportUnicorn
    {
        static ReimportUnicorn()
        {
            // 尝试重新导入Unicorn.dll
            // 原因是Unicorn.dll依赖了ugui，而如果Unicorn.dll先与ugui导入的话，会导致prefab上的scripts丢失
            var list = (from guid in AssetDatabase.FindAssets("Unicorn") where AssetDatabase.GUIDToAssetPath(guid).EndsWith("Unicorn.dll") select AssetDatabase.GUIDToAssetPath(guid)).ToList();
            if (list.Count > 0)
            {
                var assetPath = list[0];
                var md5 = FileTools.GetHexDigest16(assetPath);
                var tempFilePath = "Temp/Unicorn.dll." + md5;
                if (md5 != string.Empty && !File.Exists(tempFilePath))
                {
                    FileTools.WriteAllTextSafely(tempFilePath, string.Empty);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
            }
        }
    }
}