
/********************************************************************
created:    2014-02-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Unicorn;
using Unicorn.Collections;
using Unicorn.IO;

namespace Metadata.Menus
{
    public partial class ExportMetadata
    {
        [MenuItem(EditorMetaTools.MenuRoot + "Dispatch Metadata %#m", false, 201)]
		public static void Dispatch ()
		{
			var startTime = Time.realtimeSinceStartup;
            var manager = Export(ExportFlags.ClearBuiltFile);
			PlatformTools.DispatchFile(Constants.LocalMetadataPath);
            PlatformTools.DispatchFile(Constants.LocalIncrementMetadataPath);
            PlatformTools.DispatchFile(Constants.LocalLocaleTextPath);

			var metadataVersion = manager.GetMetadataVersion();
			var costTime = Time.realtimeSinceStartup - startTime;
			Console.WriteLine("Export metadata, metadataVersion={0}, costTime={1}", metadataVersion.ToString(), costTime.ToString("F3"));
		}

        [MenuItem(EditorMetaTools.MenuRoot + "Dispatch Increment Metadata", false, 202)]
		public static void DispatchIncrement ()
		{
            Export(ExportFlags.IncrementBuild | ExportFlags.ClearBuiltFile);
			PlatformTools.DispatchFile(Constants.LocalIncrementMetadataPath);
            PlatformTools.DispatchFile(Constants.LocalLocaleTextPath);
		}

		private static SaveAid.IncrementData _ExceptWithLastMetadata (XmlMetadataManager currentData)
		{
			var localPath = Constants.LocalMetadataPath;
			var exportPath = PathTools.GetExportPath(localPath);

            var lastStream = new FileStream(exportPath, FileMode.Open);
			var lastData = new XmlMetadataManager();
            var lastAid = lastData.LoadRawStream(lastStream, true);
			var incrementData = new SaveAid.IncrementData(lastAid.GetTexts());
			currentData.ExceptWith(lastData, ref incrementData);
			
			return incrementData;
		}

		public static MetadataManager Export (ExportFlags flags)
        {
			var clearBuiltFile = (flags & ExportFlags.ClearBuiltFile) != 0;
			if (clearBuiltFile)
			{
				var builtFile = new Build.MetadataBuiltFile();
				builtFile.Clear();
			}

			var manager = new XmlMetadataManager();
			manager.EnableXmlMetadata();

            if (null != OnExporting)
            {
                OnExporting(manager);
            }

			var isIncrementBuild = (flags & ExportFlags.IncrementBuild) != 0;
			if (isIncrementBuild)
			{
				var incrementData = _ExceptWithLastMetadata(manager);

                if (!manager.IsEmpty())
                {
                    var exportPath = PathTools.GetExportPath(Constants.LocalIncrementMetadataPath);
                    _ExportMetadata(manager, exportPath, incrementData);
                }
				else
                {
                    _ExportEmptyIncrementMetadata();
                }
			}
			else 
			{
				var exportPath = PathTools.GetExportPath(Constants.LocalMetadataPath);
				_ExportMetadata(manager, exportPath, null);
                _ExportEmptyIncrementMetadata();
			}
			
			if (null != OnExported)
			{
				OnExported(manager);
			}

			return manager;
		}
		
        private static void _ExportEmptyIncrementMetadata ()
        {
            var exportPath = PathTools.GetExportPath(Constants.LocalIncrementMetadataPath);
            // empty increment metadata@.raw will have a length= 0
            // while a real increment metadata bundle will have contents;
            FileTools.WriteAllBytesSafely(exportPath, Constants.EmptyBytes);
        }

		private static void _ExportMetadata (XmlMetadataManager manager, string exportPath, SaveAid.IncrementData incrementData)
		{
			FileTools.DeleteSafely(exportPath);
			using (var exportStream = new FileStream(exportPath, FileMode.CreateNew))
			{
				var aid = new SaveAid(incrementData);
				aid.Save(exportStream, manager, false);
			}

            var localeTextPath = PathTools.GetExportPath(Constants.LocalLocaleTextPath);
            FileTools.DeleteSafely(localeTextPath);
            using (var stream = new FileStream(localeTextPath, FileMode.CreateNew))
            {
                var isFullMode = false;
                LocaleTextManager.Instance.Save(stream, isFullMode);
            }
		}

        public static event Action<XmlMetadataManager> OnExporting;
		public static event Action<XmlMetadataManager> OnExported;
    }
}