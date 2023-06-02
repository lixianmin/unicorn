/********************************************************************
created:    2017-07-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    internal static class Logo
    {
        static Logo()
        {
            // 这个地方一定会找到Unicorn.dll中的Logo，而不是找到Unicorn.Core.dll中的自己
            // 无论是叫Assembly-CSharp.dll vs Assembly-CSharp-firstpass.dll，还是叫
            // Unicorn.dll vs. Unicorn.Core.dll，它们的排序之后顺序都不影响这个结果
            var type = TypeTools.SearchType("Logo");
            if (null != type && type != typeof(Logo))
            {
                TypeTools.CreateDelegate(type, "_WriteLine", out _lpfnWriteLine);
                TypeTools.CreateDelegate(type, "_Log", out _lpfnLogInfo);
                TypeTools.CreateDelegate(type, "_LogError", out _lpfnLogError);
                TypeTools.CreateDelegate(type, "_FormatMessage", out _lpfnFormat);
            }
            else
            {
                _lpfnWriteLine = _WriteLine;
                _lpfnLogInfo = System.Console.WriteLine;
                _lpfnLogError = System.Console.Error.WriteLine;
                _lpfnFormat = string.Format;
            }
        }

        public static void Info(string format, params object[] args)
        {
            _lpfnWriteLine(_lpfnLogInfo, _lpfnFormat(format, args));
        }

        public static void Info(string message)
        {
            _lpfnWriteLine(_lpfnLogInfo, message);
        }

        public static void Info(object message)
        {
            _lpfnWriteLine(_lpfnLogInfo, message);
        }

        public static void Error(string format, params object[] args)
        {
            _lpfnWriteLine(_lpfnLogError, _lpfnFormat(format, args));
        }

        public static void Error(string message)
        {
            _lpfnWriteLine(_lpfnLogError, message);
        }

        public static void Error(object message)
        {
            _lpfnWriteLine(_lpfnLogError, message);
        }

        private static void _WriteLine(System.Action<object> output, object message)
        {
            output(message);
        }

        private static System.Action<System.Action<object>, object> _lpfnWriteLine;
        private static System.Action<object> _lpfnLogInfo;
        private static System.Action<object> _lpfnLogError;
        private static System.Func<string, object[], string> _lpfnFormat;
    }
}