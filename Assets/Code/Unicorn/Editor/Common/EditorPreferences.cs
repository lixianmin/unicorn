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
    /// <summary>
    /// 重命名, 因为已经存在UnityEditor.EditorSettings类了
    /// </summary>
    internal static class EditorPreferences
    {
        [DidReloadScripts]
        private static void Reload()
        {
            var manifest = UnicornManifest.OpenOrCreate();
            var isChanged = _SetColorSpace(manifest);
            isChanged = _SetBakeCollisionMeshes(manifest) || isChanged;
            isChanged = _SetEnterPlayModeOptionsEnabled(manifest) || isChanged;
            isChanged = _SetManagedStrippingLevel(manifest) || isChanged;

            if (isChanged)
            {
                manifest.Save();
            }
        }

        // 设置colorSpace，默认使用Linear空间
        private static bool _SetColorSpace(UnicornManifest manifest)
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
                    return false;
                }
            }

            if (PlayerSettings.colorSpace != colorSpace)
            {
                PlayerSettings.colorSpace = colorSpace;
                Logo.Info($"(manifest.editorSettings.colorSpace={name}) => (PlayerSettings.colorSpace={name})");
                return true;
            }

            return false;
        }

        private static bool _SetBakeCollisionMeshes(UnicornManifest manifest)
        {
            var bakeCollisionMeshes = manifest.editorSettings.bakeCollisionMeshes;
            if (PlayerSettings.bakeCollisionMeshes != bakeCollisionMeshes)
            {
                PlayerSettings.bakeCollisionMeshes = bakeCollisionMeshes;
                Logo.Info(
                    $"(manifest.editorSettings.bakeCollisionMeshes={bakeCollisionMeshes}) => (PlayerSettings.bakeCollisionMeshes={bakeCollisionMeshes})");
                return true;
            }

            return false;
        }

        /// <summary>
        /// https://docs.unity3d.com/Manual/DomainReloading.html , 按文档, 这个字段似乎应该设置为true才能保证static变量在每次游戏
        /// 初始化的时候自动reset为默认值, 但实际在editor中测试的结果好像是反着的, 这个变量必须为false才可以
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        private static bool _SetEnterPlayModeOptionsEnabled(UnicornManifest manifest)
        {
            var enterPlayModeOptionsEnabled = manifest.editorSettings.enterPlayModeOptionsEnabled;
            if (EditorSettings.enterPlayModeOptionsEnabled != enterPlayModeOptionsEnabled)
            {
                EditorSettings.enterPlayModeOptionsEnabled = enterPlayModeOptionsEnabled;
                Logo.Info(
                    $"(manifest.editorSettings.enterPlayModeOptionsEnabled={enterPlayModeOptionsEnabled}) => (EditorSettings.enterPlayModeOptionsEnabled={enterPlayModeOptionsEnabled})");
                return true;
            }

            return false;
        }

        private static bool _SetManagedStrippingLevel(UnicornManifest manifest)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;

            var levels = manifest.managedStrippingLevels;
            var name = buildTarget switch
            {
                BuildTarget.Android => levels.Android,
                BuildTarget.iOS => levels.iOS,
                BuildTarget.WebGL => levels.WebGL,
                BuildTarget.StandaloneOSX => levels.StandaloneOSX,
                _ => string.Empty
            };

            if (name == string.Empty)
            {
                Logo.Error($"using unsupported buildTarget={buildTarget}");
                return false;
            }

            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var lastLevel = PlayerSettings.GetManagedStrippingLevel(targetGroup);
            var nextLevel = _GetManagedStrippingLevel(name);

            if (lastLevel != nextLevel)
            {
                PlayerSettings.SetManagedStrippingLevel(targetGroup, nextLevel);
                Logo.Info($"(manifest.editorSettings.managedStrippingLevel={name}) => ({lastLevel} → {nextLevel})");
                return true;
            }

            return false;
        }

        private static ManagedStrippingLevel _GetManagedStrippingLevel(string name)
        {
            return name switch
            {
                "Minimal" => ManagedStrippingLevel.Minimal,
                "Low" => ManagedStrippingLevel.Low,
                "Medium" => ManagedStrippingLevel.Medium,
                "High" => ManagedStrippingLevel.High,
                _ => ManagedStrippingLevel.Disabled
            };
        }
    }
}