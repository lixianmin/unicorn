/********************************************************************
created:    2023-10-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;

namespace Unicorn.Menus
{
    internal static class ReleaseModeMenu
    {
        [MenuItem(menuName, false, 208)]
        private static void _Execute()
        {
            os.isReleaseMode = !os.isReleaseMode;
            Menu.SetChecked(menuName, os.isReleaseMode);
        }

        private const string menuName = "*Tools/Release Mode";
    }
}