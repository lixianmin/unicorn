/********************************************************************
created:    2023-06-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;
using UnityEngine;

namespace Unicorn.Menus
{
    internal class GarbageCollect
    {
        [MenuItem("*Tools/Garbage Collect", false, 608)]
        private static void _Execute()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }
}