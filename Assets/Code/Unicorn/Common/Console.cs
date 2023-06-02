/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn;

// this class should not be placed into Unicorn namespace, because it will cause compile error 
// when using 'System' and 'Unicorn' simutaniously.

public static class Console
{
    public static void WriteLine(string format, params object[] args)
    {
        Logo.Info(format, args);
    }

    public static void WriteLine(string message)
    {
        Logo.Info(message);
    }

    public static void WriteLine(object message)
    {
        Logo.Info(message);
    }

    public static class Error
    {
        public static void WriteLine(string format, params object[] args)
        {
            Logo.Error(format, args);
        }

        public static void WriteLine(string message)
        {
            Logo.Error(message);
        }

        public static void WriteLine(object message)
        {
            Logo.Error(message);
        }
    }
}