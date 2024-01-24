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
        [MenuItem(menuName, false, 208)]
        private static void _Execute()
        {
            os.InternalSetIsReleaseMode(!os.isReleaseMode);
            Menu.SetChecked(menuName, os.isReleaseMode);

            // isReleaseMode在游戏开始运行的时刻, 会被reset为false
            var key = PathTools.ProjectName + ".release.mode.enabled";
            PlayerPrefs.SetInt(key, os.isReleaseMode ? 1 : 0);
        }

        private const string menuName = "*Tools/Release Mode";
    }
}