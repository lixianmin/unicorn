
/********************************************************************
created:    2018-01-23
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Diagnostics;

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
                var flag = "Revision:";
                var startIndex = text.IndexOf(flag);
                if (startIndex > 0)
                {
                    startIndex += flag.Length + 1;
                    var endIndex = text.IndexOf("\n", startIndex);
                    var revisionText = text.Substring(startIndex, endIndex - startIndex);
                    int revision;
                    int.TryParse(revisionText, out revision);

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
            si.FileName = "svn";
            si.Arguments = command + " " + directory;
            si.UseShellExecute = false;
            si.RedirectStandardOutput = true;
            process.Start();

            var text = process.StandardOutput.ReadToEnd();
            return text;
        }
    }
}