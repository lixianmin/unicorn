
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Text;

namespace Unicorn
{
    public static class Kernel
    {
        static Kernel()
        {
            isEditor = null != TypeTools.GetEditorAssembly();
        }

        public static string[] walk(string path, string searchPattern)
        {
            if (Directory.Exists(path))
            {
                var paths = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
                return paths;
            }

            return Array.Empty<string>();
        }

        public static void makedirs(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Directory.Exists(name))
            {
                var head = Path.GetDirectoryName(name);
                makedirs(head);
                Directory.CreateDirectory(name);
            }
        }

        public static void startfile(string filename, string arguments = null, bool shell = false)
        {
            var process = new System.Diagnostics.Process();
            var si = process.StartInfo;
            si.FileName = filename;
            si.Arguments = arguments;
            si.UseShellExecute = shell;
            process.Start();
        }

        public static readonly bool isEditor;
        public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
    }
}