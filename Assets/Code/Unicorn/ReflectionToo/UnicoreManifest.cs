/********************************************************************
created:    2015-03-30
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using UnityEngine;
using EditorUtility = Unicorn.Reflection.EditorUtility;

namespace Unicorn
{
    [Serializable]
    public partial class UnicornManifest
    {
        private UnicornManifest()
        {
        }

        public static UnicornManifest OpenOrCreate()
        {
            var manifestPath = _GetManifestPath();

            if (File.Exists(manifestPath))
            {
                var manifest = XmlTools.Deserialize<UnicornManifest>(manifestPath);
                return manifest;
            }
            else
            {
                var manifest = new UnicornManifest();
                XmlTools.Serialize(manifestPath, manifest);

                return manifest;
            }
        }

        private static string _GetManifestPath()
        {
            var path = os.path.join(Application.dataPath, "unicorn_manifest.xml");
            return path;
        }

        private static string _ChooseRelativePath(string title)
        {
            var dataPath = Application.dataPath;
            var folderPath = EditorUtility.OpenFolderPanel(title, dataPath, string.Empty);

            var uri1 = new Uri(dataPath);
            var uri2 = new Uri(folderPath);
            var relativePath = uri1.MakeRelativeUri(uri2).ToString();
            return relativePath;
        }

        private string _CheckChooseFullPath(ref string targetRelativePath, string title)
        {
            if (targetRelativePath.IsNullOrEmptyEx() || !Directory.Exists(targetRelativePath))
            {
                targetRelativePath = _ChooseRelativePath(title);

                if (Directory.Exists(targetRelativePath))
                {
                    var manifestPath = _GetManifestPath();
                    XmlTools.Serialize(manifestPath, this);
                }
            }
            
            var fullPath = Path.GetFullPath(targetRelativePath);
            return fullPath;
        }

        internal static string _GetEditorResourceRoot()
        {
            var manifest = OpenOrCreate();
            const string title = "Please choose the editor resource root for exporting or downloading assetBundles";
            var fullPath = manifest._CheckChooseFullPath(ref manifest.relativePaths.editorResourceRoot, title);
            return fullPath;
        }

        public static string GetXmlMetadataRoot()
        {
            var manifest = OpenOrCreate();
            const string title = "Please choose metadata xml files root directory";
            var fullPath = manifest._CheckChooseFullPath(ref manifest.relativePaths.xmlMetadataRoot, title);
            return fullPath;
        }
        
        public static string GetExportMetadataRoot()
        {
            var manifest = OpenOrCreate();
            const string title = "Please choose exported metadata root directory";
            var fullPath = manifest._CheckChooseFullPath(ref manifest.relativePaths.exportMetadataRoot, title);
            return fullPath;
        }
        
        public static string GetLuaScriptRoot()
        {
            // 我们暂时没有使用lua的需求
            return string.Empty;
        }
    }
}