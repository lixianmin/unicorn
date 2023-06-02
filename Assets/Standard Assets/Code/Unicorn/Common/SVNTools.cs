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
    public static class SVNTools
    {
        public static int GetRevision(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return 0;
            }

            try
            {
                var text = _FetchSvnCommandText("info", directory);
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

        private static string _FetchSvnCommandText(string command, string directory)
        {
            var process = new Process();
            var si = process.StartInfo;
            si.FileName = _GetSVNExecutablePath();
            si.Arguments = command + " " + directory;
            si.UseShellExecute = false; // UseShellExecute must be false in order to redirect IO streams.
            si.RedirectStandardOutput = true;

            process.Start();

            var text = process.StandardOutput.ReadToEnd();
            return text;
        }

        public static string GetExecutablePath(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var environmentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var isWindows = environmentPath.Contains(';');

            var separator = ';';
            var slash = '\\';

            if (!isWindows)
            {
                separator = ':';
                slash = '/';
                // unity3d自带的$PATH过于精简，自动补一些更接地气的
                environmentPath += ":/usr/local/bin:/opt/homebrew/bin";
            }

            var paths = environmentPath.Split(separator);
            if (paths != null)
            {
                foreach (var folder in paths)
                {
                    var filePath = folder + slash + name;
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }
                }
            }

            Logo.Error($"can not find command in $PATH={environmentPath}");
            return name;
        }

        private static string _GetSVNExecutablePath()
        {
            if (_svnExecutablePath == null)
            {
                const string name = "svn";
                _svnExecutablePath = GetExecutablePath(name);
            }

            return _svnExecutablePath;
        }

        private static string _svnExecutablePath;
    }
}