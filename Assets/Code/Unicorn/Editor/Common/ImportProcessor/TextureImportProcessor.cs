
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
            
            var isLinear = PlayerSettings.colorSpace == ColorSpace.Linear;
            if (isLinear && importer.sRGBTexture)
            {
                importer.sRGBTexture = false;
                isChanged = true;
                Console.WriteLine($"assetPath={assetPath}, (colorSpace=Linear) => (sRGBTexture=false)");
            }

            if (isChanged)
            {
                importer.SaveAndReimport();
            }
        }
        
        // private readonly UnicornManifest _manifest = UnicornManifest.OpenOrCreate();
    }
}