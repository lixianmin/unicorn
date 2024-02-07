/********************************************************************
created:    2023-10-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using UnityEngine;

namespace Unicorn.Menus
{
    internal static class ReleaseModeMenu
    {
        [MenuItem(MenuName, false, 208)]
        private static void _Execute()
        {
            os.InternalSetIsReleaseMode(!os.IsReleaseMode);
            Menu.SetChecked(MenuName, os.IsReleaseMode);

            // isReleaseMode在游戏开始运行的时刻, 会被reset为false
            PlayerPrefs.SetInt(_GetReleaseModeKey(), os.IsReleaseMode ? 1 : 0);
        }

        [MenuItem(MenuName, true)]
        static bool _ValidateReleaseMode()
        {
            if (!_isFirstTime)
            {
                _isFirstTime = true;
                
                var enabled = PlayerPrefs.GetInt(_GetReleaseModeKey(), 0);
                var isReleaseMode = enabled == 1;
                Menu.SetChecked(MenuName, isReleaseMode);
            }

            return true;
        }

        private static string _GetReleaseModeKey()
        {
            var key = PathTools.ProjectName + ".release.mode.enabled";
            return key;
        }

        private const string MenuName = "*Tools/Release Mode";
        private static bool _isFirstTime;
    }
}