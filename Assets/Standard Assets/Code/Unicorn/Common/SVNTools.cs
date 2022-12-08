
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
                Console.Error.WriteLine(ex);
            }

            return 0;
        }

        private static string _FetchSvnCommandText(string command, string directory)
        {
            var process = new Process();
            var si = process.StartInfo;
            si.FileName = _GetSVNFilePath();
            si.Arguments = command + " " + directory;
            si.UseShellExecute = false; // The Process object must have the UseShellExecute property set to false in order to redirect IO streams.
            si.RedirectStandardOutput = true;
            
            process.Start();

            var text = process.StandardOutput.ReadToEnd();
            return text;
        }

        private static string _GetSVNFilePath()
        {
            if (_svnFilePath == null)
            {
                var environmentPath = Environment.GetEnvironmentVariable("PATH")??string.Empty;
                var isWindows = environmentPath.Contains(';');
                var separator = isWindows ? ';' : ':';
                var slash = isWindows ? '\\' : '/';
                var paths = environmentPath.Split(separator);
                if (paths != null)
                {
                    foreach (var folder in paths)
                    {
                        var filePath = folder + slash + "svn";
                        if (File.Exists(filePath))
                        {
                            _svnFilePath = filePath;
                            break;
                        }
                    }
                }
            }

            if (_svnFilePath == null)
            {
                _svnFilePath = "svn";
                var environmentPath = Environment.GetEnvironmentVariable("PATH")??string.Empty;
                Console.Error.WriteLine($"can not find svn command in $PATH={environmentPath}");
            }
            
            return _svnFilePath;
        }

        private static string _svnFilePath;
    }
}