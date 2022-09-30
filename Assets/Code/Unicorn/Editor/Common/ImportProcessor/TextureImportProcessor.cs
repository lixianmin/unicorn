
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
            // https://docs.unity3d.com/2022.2/Documentation/Manual/LinearRendering-LinearTextures.html
            var mustGammaTexture = importer.textureType is TextureImporterType.Lightmap 
                or TextureImporterType.DirectionalLightmap 
                or TextureImporterType.GUI; // GUI这个实指 Editor GUI and Legacy GUI，现在实际用的GUI类型是Sprite那个 
            
            var isLinear = PlayerSettings.colorSpace == ColorSpace.Linear;
            if (isLinear)
            {
                if (importer.sRGBTexture && !mustGammaTexture)
                {
                    importer.sRGBTexture = false;
                    isChanged = true;
                    Console.WriteLine($"assetPath={assetPath}, (colorSpace=Linear) => (sRGBTexture=false)");
                } else if (!importer.sRGBTexture && mustGammaTexture)
                {
                    importer.sRGBTexture = true;
                    isChanged = true;
                    Console.WriteLine($"assetPath={assetPath}, (textureType={importer.textureType}) => (sRGBTexture=true)");  
                }
            }

            if (isChanged)
            {
                importer.SaveAndReimport();
            }
        }
        
        // private readonly UnicornManifest _manifest = UnicornManifest.OpenOrCreate();
    }
}