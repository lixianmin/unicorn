
/********************************************************************
created:    2022-09-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Unicorn
{
    internal class EditorSettings
    {
        [DidReloadScripts]
        private static void Reload()
        {
            var manifest = UnicornManifest.OpenOrCreate();
            
            // 设置colorSpace，默认使用Linear空间
            var name = manifest.editorSettings.colorSpace;
            var colorSpace = name == "Linear" ? ColorSpace.Linear : ColorSpace.Gamma;
            if (PlayerSettings.colorSpace != colorSpace)
            {
                PlayerSettings.colorSpace = colorSpace;
                Console.WriteLine($"(manifest.editorSettings.colorSpace={name}) => (PlayerSettings.colorSpace={name})");
            }
        }
    }
}