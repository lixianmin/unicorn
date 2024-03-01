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

                // FileShare指允许其它人做什么, 此处FileShare.Read是指我允许其它人read该文件. 但这个是相互的, 这意味着
                // 其它人要读取时也要允许LogFile去write本日志, 因此其它人在读的时候需要使用FileShare.Write打开, 即:
                // var readStream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.Write);
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