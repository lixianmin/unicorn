
/********************************************************************
created:    2022-09-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using UnityEngine;

namespace Unicorn
{
    internal class TextureImportProcessor : AssetPostprocessor
    {
        public void OnPreprocessTexture()
        {
            var importer = (TextureImporter) assetImporter;
            var isChanged = false;
            
            // 某些纹理必须位于gamma空间，而另外一些则倾向于是linear空间
            // 我看了texture的inspector面板，只有default的时候是可以自由勾选sRBGTexture的，这是否意味着在其它情况下无论怎么勾选的，都不影响？
            // 比如，如果是textureType是lightmap，那是否意味着无论如何都会被当作gamma空间的纹理处理？
            // https://docs.unity3d.com/2022.2/Documentation/Manual/LinearRendering-LinearTextures.html
            var isLinear = PlayerSettings.colorSpace == ColorSpace.Linear;
            if (isLinear)
            {
                var shouldLinearTexture = importer.textureType is TextureImporterType.NormalMap
                    or TextureImporterType.Sprite;
                if (importer.sRGBTexture && shouldLinearTexture)
                {
                    importer.sRGBTexture = false;
                    isChanged = true;
                    Logo.Info($"assetPath={assetPath}, (colorSpace=Linear) => (sRGBTexture=false)");
                }
            }
            
            // lightmap纹理在linear空间计算但存储在gamma空间，这一点跟colorSpace无关。所以lightmap不需要代码自动转换
            // https://docs.unity3d.com/2021.3/Documentation/Manual/LinearRendering-GammaTextures.html
            var mustGammaTexture = importer.textureType is TextureImporterType.GUI; // GUI这个实指 Editor GUI and Legacy GUI，现在实际用的Unity.GUI类型是Sprite那个 
            if (!importer.sRGBTexture && mustGammaTexture)
            {
                importer.sRGBTexture = true;
                isChanged = true;
                Logo.Info($"assetPath={assetPath}, (textureType={importer.textureType}) => (sRGBTexture=true)");  
            }

            if (isChanged)
            {
                importer.SaveAndReimport();
            }
        }
        
        // private readonly UnicornManifest _manifest = UnicornManifest.OpenOrCreate();
    }
}