/********************************************************************
created:    2018-01-23
author:     lixianmin

purpose:    svn commands
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Diagnostics;
using System.IO;

namespace Unicorn
{
    public static class SvnTools
    {
        static SvnTools()
        {
            _svnExePath = _FindSvnExePath();
        }
        
        public static int GetRevision(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return 0;
            }

            try
            {
                var text = ExecuteCommand(directory, "info");
                const string flag = "Revision:";
                var startIndex = text.IndexOf(flag, StringComparison.Ordinal);
                if (startIndex > 0)
                {
                    startIndex += flag.Length + 1;
                    var endIndex = text.IndexOf("\n", startIndex, StringComparison.Ordinal);
                    var revisionText = text.Substring(startIndex, endIndex - startIndex);
                    int.TryParse(revisionText, out var revision);

                    return revision;
                }
            }
            catch (Exception ex)
            {
                Logo.Error(ex);
            }

            return 0;
        }
        
        public static string ExecuteCommand(string workingDirectory, string arguments)
        {
            var process = new Process();

            process.StartInfo.FileName = _svnExePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.UseShellExecute = false;   // 这个在macos上必须是false
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        
        private static string _FindSvnExePath()
        {
            var svnPath = "/usr/local/bin/svn";
            if (File.Exists(svnPath))
            {
                return svnPath;
            }

            svnPath = "/usr/bin/svn";
            if (File.Exists(svnPath))
            {
                return svnPath;
            }

            return "svn";
        }

        private static readonly string _svnExePath;
    }
}