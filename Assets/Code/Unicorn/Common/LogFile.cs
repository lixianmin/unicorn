/********************************************************************
created:    2024-02-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.IO;
using Unicorn.IO;
using UnityEngine;

namespace Unicorn
{
    internal static class LogFile
    {
        internal static void Init()
        {
            try
            {
                var logPath = PathTools.LogPath;
                if (File.Exists(logPath))
                {
                    var lastLogPath = PathTools.LastLogPath;
                    FileTools.Overwrite(logPath, lastLogPath);
                }

                // 其它人要读取需要使用如下代码:
                // var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var stream = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _logWriter = new StreamWriter(stream);

                Application.logMessageReceived += _HandlerLogCallBack;
            }
            catch (Exception ex)
            {
                Logo.Error("[UnicornMain._InitLogInfo()] ex={0}", ex);
            }
        }

        private static void _HandlerLogCallBack(string logString, string stackTrace, LogType type)
        {
            // the same log should not be write twice.
            if (_lastLogString != logString)
            {
                _lastLogString = logString;

                bool needMark = false;

                if (type == LogType.Error)
                {
                    needMark = true;
                    _logs.Add("[[[Error]]]");
                }
                else if (type == LogType.Exception)
                {
                    needMark = true;
                    _logs.Add("[[[Exception]]]");
                }

                _logs.Add(logString);

                if (type == LogType.Error || type == LogType.Warning || type == LogType.Exception)
                {
                    _logs.Add(stackTrace);
                }

                if (needMark)
                {
                    _logs.Add("[[[-]]]");
                }

                _logs.Add(os.linesep);
            }
        }

        internal static void ExpensiveUpdate()
        {
            var count = _logs.Count;
            if (count > 0 && null != _logWriter)
            {
                for (var i = 0; i < count; ++i)
                {
                    var log = _logs[i];
                    _logWriter.WriteLine(log);
                }

                _logWriter.Flush();
                _logs.Clear();
            }
        }
        
        private static readonly ArrayList _logs = new();
        private static StreamWriter _logWriter;
        private static string _lastLogString;
    }
}