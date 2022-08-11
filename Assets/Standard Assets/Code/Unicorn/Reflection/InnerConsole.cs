
/********************************************************************
created:    2017-07-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using Unicorn;

internal static class Console
{
    static Console ()
    {
        // 这个地方一定会找到Uniqe.dll中的Console，而不是找到Unicorn.Core.dll中的自己
        // 无论是叫Assembly-CSharp.dll vs Assembly-CSharp-firstpass.dll，还是叫
        // Unicorn.dll vs. Unicorn.Core.dll，它们的排序之后顺序都不影响这个结果
        var type = TypeTools.SearchType("Console");
        if (null != type && type != typeof(Console))
        {
            TypeTools.CreateDelegate(type, "_WriteLine", out _lpfnWriteLine);
            TypeTools.CreateDelegate(type, "_Log", out _lpfnLog);
            TypeTools.CreateDelegate(type, "_LogError", out _lpfnLogError);
            TypeTools.CreateDelegate(type, "_FormatMessage", out _lpfnFormat);
        }
        else
        {
            _lpfnWriteLine = _WriteLine;
            _lpfnLog = System.Console.WriteLine;
            _lpfnLogError = System.Console.Error.WriteLine;
            _lpfnFormat = string.Format;
        }
    }

    public static void WriteLine (string format, params object[] args)
    {
        _lpfnWriteLine(_lpfnLog, _lpfnFormat(format, args));
    }

    public static void WriteLine (string message)
    {
        _lpfnWriteLine(_lpfnLog, message);
    }

    public static void WriteLine (object message)
    {
        _lpfnWriteLine(_lpfnLog, message);
    }

    private static void _WriteLine (System.Action<object> output, object message)
    {
        output(message);
    }

    public static class Error
    {
        public static void WriteLine (string format, params object[] args)
        {
            _lpfnWriteLine(_lpfnLogError, _lpfnFormat(format, args));
        }

        public static void WriteLine (string message)
        {
            _lpfnWriteLine(_lpfnLogError, message);
        }

        public static void WriteLine (object message)
        {
            _lpfnWriteLine(_lpfnLogError, message);
        }
    }

    private static System.Action<System.Action<object>, object> _lpfnWriteLine;
    private static System.Action<object> _lpfnLog;
    private static System.Action<object> _lpfnLogError;
    private static System.Func<string, object[], string> _lpfnFormat;
}