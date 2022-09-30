
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
            _SetColorSpace(manifest);
        }

        // 设置colorSpace，默认使用Linear空间
        private static void _SetColorSpace(UnicornManifest manifest)
        {
            var name = manifest.editorSettings.colorSpace;
            var colorSpace = name == "Linear" ? ColorSpace.Linear : ColorSpace.Gamma;
            if (colorSpace == ColorSpace.Linear)
            {
                // 只有以下平台是支持Linear空间的
                // 参考：https://docs.unity3d.com/2022.2/Documentation/Manual/LinearRendering-GammaTextures.html
                var isValidBuildTarget = false;
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneOSX:
                    case BuildTarget.StandaloneLinux64:
                    case BuildTarget.Android:
                    case BuildTarget.iOS:
                    case BuildTarget.WebGL:
                        isValidBuildTarget = true;
                        break;
                }

                if (!isValidBuildTarget)
                {
                    return;
                }
            }
            
            if (PlayerSettings.colorSpace != colorSpace)
            {
                PlayerSettings.colorSpace = colorSpace;
                Console.WriteLine($"(manifest.editorSettings.colorSpace={name}) => (PlayerSettings.colorSpace={name})");
            }
        }
    }
}