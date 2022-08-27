
/********************************************************************
created:    2022-08-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unicorn.Kit
{
    internal class KitMenus
    {
        [MenuItem("*Kit/Make Auto Code", false, 0)]
        private static void Make ()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Warning", "Wait for compiling", "OK");
                return;
            }

            var factoryPath = _GetFactoryFilePath();
            os.makedirs(Path.GetDirectoryName(factoryPath));
            new KitAutoCode().WriteKitFactory(factoryPath);
        }

        [MenuItem("*Kit/Clear Auto Code", false, 1)]
        public static void Clear ()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Warning", "Wait for compiling", "OK");
                return;
            }

            var factoryPath = _GetFactoryFilePath();
            if (File.Exists(factoryPath))
            {
                File.Delete(factoryPath);
                AssetDatabase.Refresh();
            }
        }

        private static string _GetFactoryFilePath()
        {
            var factoryPath = Application.dataPath + "/Code/Unicorn/Kit/_KitFactory.cs";
            return factoryPath;
        }
    }
}