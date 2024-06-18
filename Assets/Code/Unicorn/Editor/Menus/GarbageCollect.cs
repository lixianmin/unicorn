/********************************************************************
created:    2023-06-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Web.Internal;
using UnityEditor;
using UnityEngine;

namespace Unicorn.Menus
{
    internal static class GarbageCollect
    {
        [MenuItem("*Tools/Garbage Collect", false, 608)]
        private static void _Execute()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            PrefabRecycler.PrintSummary();
        }
    }
}