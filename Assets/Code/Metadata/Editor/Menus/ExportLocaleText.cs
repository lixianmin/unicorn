
/********************************************************************
created:    2017-07-25
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using Unicorn;
using Unicorn.IO;
using System.IO;

namespace Metadata.Menus
{
    public class ExportLocaleText
    {
        [MenuItem(EditorMetaTools.MenuRoot + "Dispatch I18N", false, 205)]
		public static void Dispatch ()
		{
            const string title = "Choose locale xml files folder, folder name will be the exported locale file name.";
            var dataPath = PathTools.EditorResourceRoot;
            var folderPath = EditorUtility.OpenFolderPanel(title, dataPath, string.Empty);
            
            if (string.IsNullOrEmpty(folderPath)|| !Directory.Exists(folderPath))
            {
                Logo.Error($"invalid folderPath={folderPath}, just ignore.");
                return;    
            }

			var startTime = Time.realtimeSinceStartup;

            var builtFile = new Metadata.Build.MetadataBuiltFile();
            builtFile.LoadLocaleText();

            var localeCount = 0;
            var localeTable = new LocaleTable();
            var filePaths = os.walk(folderPath, "*.xml");
            Logo.Info($"Scan *.xml in foldPath={folderPath}, filePaths.Length={filePaths.Length}");

            foreach (var filepath in filePaths)
            {
                var translator = XmlTools.Deserialize<XmlLocaleTextTranslator>(filepath);
                foreach (var item in translator.items)
                {
                    var guid = item.GetGUID();

                    if (LocaleTextManager.It.Contains(guid))
                    {
                        localeTable.Add(guid, item.text, false);
                        ++localeCount;
                    }
                }
            }

            LocaleTextManager.It.AddLocaleTable(localeTable);
            
            // folderPath就是导出的本地化文件的名字
            var name = Path.GetFileName(folderPath) + ".bytes";
            var localeTextPath = os.path.join(UnicornManifest.GetExportMetadataRoot(), name);
            FileTools.DeleteSafely(localeTextPath);
            using (var stream = new FileStream(localeTextPath, FileMode.CreateNew))
            {
                LocaleTextManager.It.Save(stream, false);
            }

            // PlatformTools.DispatchFile(localeTextPath);

			var costTime = Time.realtimeSinceStartup - startTime;
            var totalCount = LocaleTextManager.It.GetCount();

            Logo.Info($"Dispatch locale texts (I18N), filename={name}, localeRatio={localeCount}/{totalCount}, costTime={costTime:F3}");
		}
    }
}