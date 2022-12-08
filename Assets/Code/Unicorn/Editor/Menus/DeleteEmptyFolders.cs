
/********************************************************************
created:    2016-05-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace Unicorn.Menus
{
    internal class DeleteEmptyFolders
    {
        [MenuItem("*Metadata/Delete Empty Folders", false, 604)]
        private static void _Execute ()
        {
            var sbLog = new StringBuilder("Deleted empty folders:\n");

            var topDir = Application.dataPath;
            var directories = Directory.GetDirectories(topDir);
            foreach (var dirPath in directories)
            {
                _CheckDeleteEmptyFolders(dirPath, sbLog);
            }

            var log = sbLog.ToString();
            Console.WriteLine(log);
        }

        private static bool _CheckDeleteEmptyFolders (string dirpath, StringBuilder sbLog)
        {
            var hasFiles = false;
            var subDirs = Directory.GetDirectories(dirpath);
            foreach (var subDirPath in subDirs)
            {
                hasFiles = _CheckDeleteEmptyFolders(subDirPath, sbLog) || hasFiles;
            }

            if (!hasFiles && _IsEmptyFolder(dirpath))
            {
                _DeleteEmptyFolder(dirpath, sbLog);
                return false;
            }

            return true;
        }

        private static void _DeleteEmptyFolder (string dirpath, StringBuilder sbLog)
        {
            Directory.Delete(dirpath, true);
            var metaFilePath = dirpath + ".meta";
            if (File.Exists(metaFilePath))
            {
                File.Delete(metaFilePath);
            }

            sbLog.AppendLine(dirpath);
        }

        private static bool _IsEmptyFolder (string dirpath)
        {
            var subFiles = Directory.GetFiles(dirpath);
            if (subFiles.Length == 0)
            {
                return true;
            }

            if (subFiles.Length == 1 && subFiles[0].EndsWith(".DS_Store"))
            {
                return true;
            }

            return false;
        }
    }
}