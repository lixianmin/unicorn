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
            os.isReleaseMode = !os.isReleaseMode;
            Menu.SetChecked(menuName, os.isReleaseMode);

            // isReleaseMode在游戏开始运行的时刻, 会被reset为false
            PlayerPrefs.SetInt("release.mode.enabled", os.isReleaseMode ? 1 : 0);
        }

        private const string menuName = "*Tools/Release Mode";
    }
}