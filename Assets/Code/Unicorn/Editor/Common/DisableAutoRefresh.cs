/********************************************************************
created:    2022-08-26
author:     lixianmin

参考: https://forum.unity.com/threads/disable-auto-refresh-when-in-playmode.136325/

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;

namespace Unicorn
{
    [InitializeOnLoad]
    internal class DisableAutoRefresh
    {
        static DisableAutoRefresh()
        {
            EditorApplication.playModeStateChanged += _OnPlayModeStateChanged;
            // var willPlay = EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private static void _OnPlayModeStateChanged(PlayModeStateChange playingState)
        {
            switch (playingState)
            {
                // Called the moment after the user presses the Play button.
                case PlayModeStateChange.ExitingEditMode:
                    break;
                
                // Called when the initial scene is loaded and first rendered, after ExitingEditMode..
                case PlayModeStateChange.EnteredPlayMode:
                    // 把ExitingEditMode这个时机留出来给其它代码使用
                    // 也可以手动关闭Preference->AssetPipeline->AutoRefresh 禁用改动代码后AutoRefresh
                    if (EditorPrefs.HasKey("kAutoRefresh"))
                    {
                        EditorPrefs.SetBool("kAutoRefresh", false);    
                    }
                    
                    // 进入play模式之前, 自动Refresh一把, 以加载正确的csharp代码
                    EditorApplication.ExecuteMenuItem("Assets/Refresh");
                    break;
 
                // Called the moment after the user presses the Stop button.
                case PlayModeStateChange.ExitingPlayMode:
                    break;
 
                // Called after the current scene is unloaded, after ExitingPlayMode.
                case PlayModeStateChange.EnteredEditMode:
                    break;
            }
        }
    }
}