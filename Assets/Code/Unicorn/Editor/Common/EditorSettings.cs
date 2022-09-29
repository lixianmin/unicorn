
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
    public class EditorSettings
    {
        [DidReloadScripts]
        private static void Reload()
        {
            var manifest = UnicornManifest.OpenOrCreate();
            // 设置colorSpace，默认使用Linear空间
            PlayerSettings.colorSpace = manifest.editorSettings.colorSpace == "Linear" ? ColorSpace.Linear : ColorSpace.Gamma;
        }
    }
}