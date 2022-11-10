
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
        // todo 这个流程需要重新测试来过
        // [MenuItem(EditorMetaTools.MenuRoot + "Dispatch I18N", false, 205)]
		public static void Dispatch ()
		{
            var title = "Choose locale text directory.";
            var dataPath = PathTools.EditorResourceRoot;
            var folderPath = EditorUtility.OpenFolderPanel(title, dataPath, string.Empty);

            if (string.IsNullOrEmpty(folderPath))
            {
                Console.WriteLine("No folder selected, just ignore.");
                return;    
            }

			var startTime = Time.realtimeSinceStartup;

            var builtFile = new Metadata.Build.MetadataBuiltFile();
            builtFile.LoadLocaleText();

            var localeCount = 0;
            var localeTable = new LocaleTable();
            var filePaths = os.walk(folderPath, "*.xml");

            foreach (var filepath in filePaths)
            {
                var translator = XmlTools.Deserialize<XmlLocaleTextTranslator>(filepath);
                foreach (var item in translator.items)
                {
                    var guid = item.GetGUID();

                    if (LocaleTextManager.Instance.Contains(guid))
                    {
                        localeTable.Add(guid, item.text, false);
                        ++localeCount;
                    }
                }
            }

            LocaleTextManager.Instance.AddLocaleTable(localeTable);

            var name = Path.GetFileName(folderPath) + ".bytes";
            var localeTextPath = PathTools.GetExportPath(name);
            FileTools.DeleteSafely(localeTextPath);
            using (var stream = new FileStream(localeTextPath, FileMode.CreateNew))
            {
                LocaleTextManager.Instance.Save(stream, false);
            }

            PlatformTools.DispatchFile(localeTextPath);

			var costTime = Time.realtimeSinceStartup - startTime;
            var totalCount = LocaleTextManager.Instance.GetCount();

            Console.WriteLine("Dispatch locale texts (I18N), filename={0}, localeRatio={1}/{2}, costTime={3}"
                , name, localeCount.ToString(), totalCount.ToString(), costTime.ToString("F3"));
		}
    }
}