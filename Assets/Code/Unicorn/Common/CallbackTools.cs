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
                    Logo.Warn("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}", title, ex, ex.StackTrace);
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
                    Logo.Warn("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}", title, ex, ex.StackTrace);
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
                    Logo.Warn("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}\n\nself={3}", title, ex,
                        ex.StackTrace, self);
                }
                finally
                {
                    handler = null; // handler是一个回调，它可能间接引用了大量对象，及时置空;
                }
            }
        }

        public static void Handle<T1, T2>(ref Action<T1, T2> handler, T1 self1, T2 self2, string title = "")
        {
            if (handler != null)
            {
                try
                {
                    handler(self1, self2);
                }
                catch (Exception ex)
                {
                    Logo.Warn("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}\n\nself1={3}, self2={4}", title, ex,
                        ex.StackTrace, self1, self2);
                }
                finally
                {
                    handler = null;
                }
            }
        }
    }
}