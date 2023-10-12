/********************************************************************
created:    2023-10-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;

namespace Unicorn.Menus
{
    internal static class ReleasedFlowMenu
    {
        [MenuItem(menuName, false, 208)]
        private static void _Execute()
        {
            os.isReleasedFlow = !os.isReleasedFlow;
            Menu.SetChecked(menuName, os.isReleasedFlow);
        }

        private const string menuName = "*Tools/Released Flow";
    }
}