
/********************************************************************
created:    2022-08-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unicorn
{
    internal class KitMenus
    {
        [MenuItem("*Kit/Make Auto Code", true)]
        private static bool ValidateMake ()
        {
            var manifest = UnicornManifest.OpenOrCreate();
            var isValid = manifest.makeAutoCode && !EditorApplication.isCompiling;
            return isValid;
        }
        
        [MenuItem("*Kit/Make Auto Code", false, 0)]
        private static void Make ()
        {
            // make auto code之前先Refresh()一把, 因为通常只所以需要make auto code就是因为加入了新的类型, 这时如果不Refresh()一把就找不到新的类型
            AssetDatabase.Refresh();
            
            var factoryPath = _GetFactoryFilePath();
            os.makedirs(Path.GetDirectoryName(factoryPath));
            new KitAutoCode().WriteKitFactory(factoryPath);
            
            AssetDatabase.Refresh();
        }

        [MenuItem("*Kit/Clear Auto Code", true)]
        private static bool ValidateClear()
        {
            var manifest = UnicornManifest.OpenOrCreate();
            var isValid = manifest.makeAutoCode && !EditorApplication.isCompiling;
            return isValid;
        }

        [MenuItem("*Kit/Clear Auto Code", false, 1)]
        public static void Clear ()
        {
            var factoryPath = _GetFactoryFilePath();
            if (File.Exists(factoryPath))
            {
                File.Delete(factoryPath);
                
                // Delete()之后Refresh()一把, 因为通常只所以需要Clear就是因为编译出错了, 这时如果不Refresh()一把不会修正编译问题
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