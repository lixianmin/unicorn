/********************************************************************
created:    2014-02-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Unicorn;
using Unicorn.IO;

namespace Metadata.Menus
{
    public class ExportMetadata
    {
        [MenuItem(EditorMetaTools.MenuRoot + "Dispatch Metadata %&m", false, 201)]
        public static void Dispatch()
        {
            var startTime = Time.realtimeSinceStartup;
            var manager = Export(ExportFlags.ClearBuiltFile);
            // PlatformTools.DispatchFile(Constants.LocalMetadataPath);
            // PlatformTools.DispatchFile(Constants.LocalIncrementMetadataPath);
            // PlatformTools.DispatchFile(Constants.LocalLocaleTextPath);

            var metadataVersion = manager.GetMetadataVersion();
            var costTime = Time.realtimeSinceStartup - startTime;
            Logo.Info($"Export metadata, metadataVersion={metadataVersion.ToString()}, costTime={costTime:F3}");
        }

        [MenuItem(EditorMetaTools.MenuRoot + "Dispatch Increment Metadata", false, 202)]
        public static void DispatchIncrement()
        {
            Export(ExportFlags.IncrementBuild | ExportFlags.ClearBuiltFile);
            // PlatformTools.DispatchFile(Constants.LocalIncrementMetadataPath);
            // PlatformTools.DispatchFile(Constants.LocalLocaleTextPath);
        }

        private static SaveAid.IncrementData _ExceptWithLastMetadata(XmlMetadataManager currentData)
        {
            var exportPath = _GetExportMetadataPath(Constants.LocalMetadataPath);

            var lastStream = new FileStream(exportPath, FileMode.Open);
            var lastData = new XmlMetadataManager();
            var lastAid = lastData.LoadRawStream(lastStream, true);
            var incrementData = new SaveAid.IncrementData(lastAid.GetTexts());
            currentData.ExceptWith(lastData, ref incrementData);

            return incrementData;
        }

        public static MetadataManager Export(ExportFlags flags)
        {
            var clearBuiltFile = (flags & ExportFlags.ClearBuiltFile) != 0;
            if (clearBuiltFile)
            {
                var builtFile = new Build.MetadataBuiltFile();
                builtFile.Clear();
            }

            var manager = new XmlMetadataManager();
            manager.EnableXmlMetadata();

            OnExporting?.Invoke(manager);

            var isIncrementBuild = (flags & ExportFlags.IncrementBuild) != 0;
            if (isIncrementBuild)
            {
                var incrementData = _ExceptWithLastMetadata(manager);

                if (!manager.IsEmpty())
                {
                    var exportPath = _GetExportMetadataPath(Constants.LocalIncrementMetadataPath);
                    _ExportMetadata(manager, exportPath, incrementData);
                }
                else
                {
                    _ExportEmptyIncrementMetadata();
                }
            }
            else
            {
                var exportPath = _GetExportMetadataPath(Constants.LocalMetadataPath);
                _ExportMetadata(manager, exportPath, null);
                _ExportEmptyIncrementMetadata();
            }

            OnExported?.Invoke(manager);
            return manager;
        }

        private static string _GetExportMetadataPath(string localPath)
        {
            var root = UnicornManifest.GetExportMetadataRoot();
            return os.path.join(root, localPath);
        }

        private static void _ExportEmptyIncrementMetadata()
        {
            var exportPath = _GetExportMetadataPath(Constants.LocalIncrementMetadataPath);
            // empty increment metadata@.bytes will have a length= 0
            // while a real increment metadata bundle will have contents;
            FileTools.WriteAllBytesSafely(exportPath, Constants.EmptyBytes);
        }

        private static void _ExportMetadata(MetadataManager manager, string exportPath, SaveAid.IncrementData incrementData)
        {
            FileTools.DeleteSafely(exportPath);
            using (var exportStream = new FileStream(exportPath, FileMode.CreateNew))
            {
                var aid = new SaveAid(incrementData);
                aid.Save(exportStream, manager, false);
            }

            var localeTextPath = _GetExportMetadataPath(Constants.LocalLocaleTextPath);
            FileTools.DeleteSafely(localeTextPath);
            using (var stream = new FileStream(localeTextPath, FileMode.CreateNew))
            {
                const bool isFullMode = false;
                LocaleTextManager.It.Save(stream, isFullMode);
            }
            
            Logo.Info($"[_ExportMetadata()] exportPath={exportPath}");
        }

        public static event Action<XmlMetadataManager> OnExporting;
        public static event Action<XmlMetadataManager> OnExported;
    }
}