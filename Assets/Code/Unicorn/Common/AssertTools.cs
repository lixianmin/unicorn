
/********************************************************************
created:    2022-08-11
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Diagnostics;

namespace Unicorn
{
    public static class AssertTools
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue(bool condition)
        {
            IsTrue(condition, "An error occured");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                Console.Error.WriteLine(message);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!expected.Equals(actual))
            {
                var message = $"expected = {expected.ToString()}, actual={actual.ToString()}";
                Console.Error.WriteLine(message);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNotNull<T>(T obj)
        {
            if (null == obj)
            {
                var message = $"obj is null, type = {typeof(T)}";
                Console.Error.WriteLine(message);
            }
        }

        //		[System.Diagnostics.Conditional("UNITY_EDITOR")]
        //        public static void IsNotNull<T> (T obj, string format, params object[] args)
        //        {
        //            if (null == obj)
        //            {
        //                var message = string.Format(null, format, args);
        //				Console.Error.WriteLine("obj is null, type = {0}, message={1}", typeof(T), message);
        //            }
        //        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void IsNull<T>(T obj)
        {
            if (null != obj)
            {
                Console.Error.WriteLine("obj should be null, type = {0}", typeof(T));
            }
        }

        //		[System.Diagnostics.Conditional("UNITY_EDITOR")]
        //        public static void IsNull<T> (T obj, string format, params object[] args)
        //        {
        //            if (null != obj)
        //            {
        //                var message = string.Format(null, format, args);
        //				Console.Error.WriteLine("obj should be null, type = {0}, message={1}", typeof(T), message);
        //            }
        //        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Greater(int a, int b)
        {
            if (a <= b)
            {
                Console.Error.WriteLine("Expected: a > b, given: {0} <= {1}", a.ToString(), b.ToString());
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        internal static void BeginTimeout()
        {
            var stopwatch = _GetStopwatch();
            stopwatch.Reset();
            stopwatch.Start();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        internal static void EndTimeoutException(float timeout)
        {
            var stopwatch = _GetStopwatch();
            stopwatch.Stop();

            var handleTime = stopwatch.ElapsedMilliseconds * 0.001f;

            if (handleTime > timeout)
            {
                var message = $"cost={handleTime:F3} s, expected={timeout:F3} s";
                throw new TimeoutException(message);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        internal static void EndTimeoutLog(float timeout, string message)
        {
            var stopwatch = _GetStopwatch();
            stopwatch.Stop();

            var handleTime = stopwatch.ElapsedMilliseconds * 0.001f;

            if (handleTime > timeout)
            {
                Console.Error.WriteLine($"cost={handleTime:F3} s, expected={timeout:F3} s, message={message}");
            }

            BeginTimeout();
        }

        private static Stopwatch _GetStopwatch()
        {
            return _stopwatch ??= new Stopwatch();
        }

        private static Stopwatch _stopwatch;
    }
}