/********************************************************************
created:    2023-06-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Text;
using System.Threading;

namespace Unicorn
{
    [Flags]
    public enum LogoFlags : ushort
    {
        None = 0x00,
        DetailedMessage = 0x01,

        // OpenStandardOutput = 0x02,
        FlushOnWrite = 0x04,
        LogDebug = 0x08, // 是否启用debug级别的日志
    }

    public static class Logo
    {
        static Logo()
        {
            _idMainThread = Thread.CurrentThread.ManagedThreadId;
            Flags = LogoFlags.DetailedMessage;
        }

        internal static void Init()
        {
            // 编辑器中, 启用每次输出log, 方便开发过程中调试
            if (!os.IsReleaseMode)
            {
                Flags |= LogoFlags.FlushOnWrite;
            }
        }

        internal static void ExpensiveUpdate()
        {
            _time = UnityEngine.Time.realtimeSinceStartup;
            _frameCount = UnityEngine.Time.frameCount;
            _CheckFlushLogText();
        }

        public static void Debug(string format, params object[] args)
        {
            if (_HasFlags(LogoFlags.LogDebug))
            {
                _WriteLine(_lpfnLogInfo, _FormatMessage(format, args));
            }
        }

        public static void Debug(string message)
        {
            if (_HasFlags(LogoFlags.LogDebug))
            {
                _WriteLine(_lpfnLogInfo, message);
            }
        }

        public static void Debug(object message)
        {
            if (_HasFlags(LogoFlags.LogDebug))
            {
                _WriteLine(_lpfnLogInfo, message);
            }
        }

        public static void Info(string format, params object[] args)
        {
            _WriteLine(_lpfnLogInfo, _FormatMessage(format, args));
        }

        public static void Info(string message)
        {
            _WriteLine(_lpfnLogInfo, message);
        }

        public static void Info(object message)
        {
            _WriteLine(_lpfnLogInfo, message);
        }

        public static void Warn(string format, params object[] args)
        {
            _WriteLine(_lpfnLogWarn, _FormatMessage(format, args));
        }

        public static void Warn(string message)
        {
            _WriteLine(_lpfnLogWarn, message);
        }

        public static void Warn(object message)
        {
            _WriteLine(_lpfnLogWarn, message);
        }

        public static void Error(string format, params object[] args)
        {
            _WriteLine(_lpfnLogError, _FormatMessage(format, args));
        }

        public static void Error(string message)
        {
            _WriteLine(_lpfnLogError, message);
        }

        public static void Error(object message)
        {
            _WriteLine(_lpfnLogError, message);
        }

        private static void _WriteLine(Action<object> output, object message)
        {
            // var isMainThread = Thread.CurrentThread.ManagedThreadId == _idMainThread;

            if (_HasFlags(LogoFlags.DetailedMessage))
            {
                // int frameCount;
                // string time;
                //
                // if (isMainThread)
                // {
                // 	// 以下两行代码不能在MonoBehaviour的构造方法中调用
                //  // 发现AndroidLogcatInternalLog插件，会在非主线程中调用到下面这行代码，所以只能先注释掉了
                // 	frameCount = UnityEngine.Time.frameCount;
                // 	time = UnityEngine.Time.realtimeSinceStartup.ToString("F3");
                // }
                // else
                // {
                // 	frameCount = os.frameCount;
                // 	time = _time.ToString("F3");
                // }

                var frameCount = _frameCount;
                if (_lastFrameCount != frameCount)
                {
                    _lastFrameCount = frameCount;
                    _messageFormat[1] = frameCount.ToString();
                }

                _messageFormat[3] = _time.ToString("F2");
                _messageFormat[5] = null != message ? message.ToString() : "null message (-_-)";
                message = string.Concat(_messageFormat);
            }

            try
            {
                // 现在Debug.Log()方法可以在非主线程中调用了，不再需要迂回的手段了
                output(message);
            }
            catch (MissingMethodException)
            {
                System.Console.WriteLine(message);
            }
        }

        private static string _FormatMessage(string format, params object[] args)
        {
            if (args.IsNullOrEmpty())
            {
                return format;
            }

            var message = "null format (-__-)";
            if (null != format)
            {
                _sbFormatter.AppendFormat(null, format, args);
                message = _sbFormatter.ToString();
                _sbFormatter.Length = 0;
            }

            return message;
        }

        private static void _LogInfo(object message)
        {
            if (_HasFlags(LogoFlags.FlushOnWrite))
            {
                UnityEngine.Debug.Log(message);
            }
            else
            {
                _sbLogText.AppendLine(message.ToString());
            }

            // if (_HasFlags(LogoFlags.OpenStandardOutput))
            // {
            //     System.Console.WriteLine(message);
            // }
        }

        private static void _LogWarn(object message)
        {
            _CheckFlushLogText();
            UnityEngine.Debug.LogWarning(message);

            // if (_HasFlags(LogoFlags.OpenStandardOutput))
            // {
            //     System.Console.WriteLine(message);
            // }
        }

        private static void _LogError(object message)
        {
            _CheckFlushLogText();
            UnityEngine.Debug.LogError(message);

            // if (_HasFlags(LogoFlags.OpenStandardOutput))
            // {
            //     System.Console.Error.WriteLine(message);
            // }
        }

        private static void _CheckFlushLogText()
        {
            if (_sbLogText.Length > 0)
            {
                UnityEngine.Debug.Log(_sbLogText);
                _sbLogText.Length = 0;
            }
        }

        private static bool _HasFlags(LogoFlags flags)
        {
            return (_flags & flags) != 0;
        }

        public static LogoFlags Flags
        {
            get => _flags;
            set
            {
                if (_flags == value)
                {
                    return;
                }

                _flags = value;

                if (_HasFlags(LogoFlags.FlushOnWrite))
                {
                    _CheckFlushLogText();
                }
            }
        }

        private static LogoFlags _flags;

        private static readonly int _idMainThread;
        private static float _time;
        private static int _frameCount;
        private static int _lastFrameCount;

        private static readonly StringBuilder _sbLogText = new(512);
        private static readonly StringBuilder _sbFormatter = new();

        private static readonly Action<object> _lpfnLogInfo = _LogInfo;
        private static readonly Action<object> _lpfnLogWarn = _LogWarn;
        private static readonly Action<object> _lpfnLogError = _LogError;

        private static readonly string[] _messageFormat =
        {
            "[@@frame=", // 加两个@号, 方便使用 adb logcat | grep @@ 过滤日志
            "(-_-)",
            ", time=",
            null,
            "] ",
            null
        };
    }
}