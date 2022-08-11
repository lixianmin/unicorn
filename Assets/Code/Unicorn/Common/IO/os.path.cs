
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using System.Collections.Generic;

namespace Unicorn
{
    public static partial class os
    {
        public static class path
        {
            public static string join(string path1, string path2)
            {
                if (string.IsNullOrEmpty(path1))
                {
                    return path2;
                }

                if (string.IsNullOrEmpty(path2))
                {
                    return path1;
                }

                if (_IsRootChar(path2[0]))
                {
                    return path2;
                }

                var c = path1[path1.Length - 1];
                if (!_IsRootChar(c))
                {
                    return path1 + '/' + path2;
                }

                return path1 + path2;
            }

            private static bool _IsRootChar(char c)
            {
                return c == Path.DirectorySeparatorChar
                        || c == Path.AltDirectorySeparatorChar
                        || c == Path.VolumeSeparatorChar;
            }

            public static string join(string path1, string path2, string path3)
            {
                return join(path1, join(path2, path3));
            }

            public static long getsize(string filename)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        var size = fileInfo.Length;
                        return size;
                    }
                }
                catch (Exception)
                {

                }

                return 0;
            }

            public static DateTime getctime(string filename)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        var time = fileInfo.CreationTime;
                        return time;
                    }
                }
                catch (Exception)
                {

                }

                return new DateTime();
            }

            public static DateTime getmtime(string filename)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        var time = fileInfo.LastWriteTime;
                        return time;
                    }
                }
                catch (Exception)
                {

                }

                return new DateTime();
            }

            public static string normpath(string path)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    return path.Replace('\\', '/');
                }

                return string.Empty;
            }

            public static void normpath(IList<string> paths)
            {
                if (null != paths)
                {
                    var count = paths.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        paths[i] = normpath(paths[i]);
                    }
                }
            }
        }
    }
}
