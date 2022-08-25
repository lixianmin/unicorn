
/********************************************************************
created:    2014-10-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Diagnostics;

namespace Unicorn
{
    public static class CallbackTools
    {
        public static void Handle(Action handler, string title)
        {
            if (null != handler)
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}"
                        , title, ex, ex.StackTrace);
                }
            }
        }
        
        public static void Handle(ref Action handler, string title)
        {
            if (null != handler)
            {
                try
                {
                    //AssertTools.BeginTimeout();
                    handler();
                    //AssertTools.EndTimeout(Timeout);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[CallbackTools.Handle()] {0}, ex= {1},\n\n StackTrace={2}"
                                            , title, ex, ex.StackTrace);
                }
                finally
                {
                    handler = null; // handler是一个回调，它可能间接引用了大量对象，及时置空;
                }
            }
        }

        public static void Handle<T>(ref Action<T> handler, T self, string title)
        {
            Handle(ref handler, self, title, string.Empty);
        }
        
        public static void Handle<T, U>(ref Action<T> handler, T self, string title, U text)
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
                    Console.Error.WriteLine("[CallbackTools.Handle()] {0} {1}, ex= {2},\n\n StackTrace={3},\n\n this= {4}"
                                            , title, text.ToString(), ex.ToString(), ex.StackTrace, self);
                }
                finally
                {
                    handler = null; // handler是一个回调，它可能间接引用了大量对象，及时置空;
                }
            }
        }
        
        //        public static float Timeout = 5f;
    }
}
