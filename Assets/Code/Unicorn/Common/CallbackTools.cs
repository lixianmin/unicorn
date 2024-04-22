/********************************************************************
created:    2014-10-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public static class CallbackTools
    {
        public static void Handle(Action handler, string title = "")
        {
            if (null != handler)
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Logo.Warn($"[CallbackTools.Handle()] {title}, ex= {ex},\n\n StackTrace={ex.StackTrace}");
                }
            }
        }

        public static void Handle(ref Action handler, string title = "")
        {
            if (null != handler)
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Logo.Warn($"[CallbackTools.Handle()] {title}, ex= {ex},\n\n StackTrace={ex.StackTrace}");
                }
                finally
                {
                    handler = null; // handler是一个回调，它可能间接引用了大量对象，及时置空;
                }
            }
        }

        public static void Handle<T>(ref Action<T> handler, T self, string title = "")
        {
            if (null != handler)
            {
                try
                {
                    //AssertTools.BeginTimeout();
                    handler(self);
                    //AssertTools.EndTimeout(Timeout);
                }
                catch (Exception ex)
                {
                    Logo.Warn(
                        $"[CallbackTools.Handle()] {title}, ex= {ex},\n\n StackTrace={ex.StackTrace},\n\n this= {self}");
                }
                finally
                {
                    handler = null; // handler是一个回调，它可能间接引用了大量对象，及时置空;
                }
            }
        }
    }
}