
/********************************************************************
created:    2014-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using Metadata.Build;

namespace Metadata.Menus
{
    static class RebuildMetadata
    {
//        [MenuItem(EditorMetaCommon.MenuRoot + "Rebuild *.xml metadata", false, 110)]
		public static void Make ()
        {
			var builtFile = new MetadataBuiltFile();
			builtFile.Clear();
			builtFile.Build();
        }
    }
}