
/********************************************************************
created:    2015-01-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn;

namespace Unicorn.IO
{
    public static class ScanTools
    {
        public static bool ScanAll (string title, string[] paths, Action<string> handler)
        {
            if (paths.IsNullOrEmptyEx() || null == handler)
            {
                return false;
            }

            title = title ?? string.Empty;
            var length = paths.Length;

            var invLength = 1.0f / length;
			var startTime = System.DateTime.Now;

			var lastProgress = 0.0f;
            var showPath = string.Empty;

            try
            {
                for (int i= 0; i< length; ++i)
                {
                    var path = paths[i];

					// Dont need change DisplayCancelableProgressBar() rapidly, it will slow down
					// the scanning time greatly.

                    var progress = i * invLength;
					if (progress - lastProgress < 0.1f)
					{
						progress = lastProgress;
					}
					else
					{
						lastProgress = progress;
                        showPath = path;
					}

                    var isCanceled = _DisplayCancelableProgressBar(title, showPath, progress);
					if (isCanceled)
					{
						return false;
					}

                    handler(path);
                }
            }
            finally
            {
                _ClearProgressBar();
            }

			var timeSpan = System.DateTime.Now - startTime;
			Console.WriteLine("[ScanTools.ScanAll()] {0}, costTime={1}s", title, timeSpan.TotalSeconds.ToString("F2"));
            return true;
        }

        private static Func<string, string, float, bool> _lpfnDisplayCancelableProgressBar;
        private static bool _DisplayCancelableProgressBar (string title, string info, float progress)
        {
            if (!_IsEditor())
            {
                return false;
            }

            if (null == _lpfnDisplayCancelableProgressBar)
            {
                var type = System.Type.GetType("UnityEditor.EditorUtility,UnityEditor");
                var method = type.GetMethod("DisplayCancelableProgressBar", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                TypeTools.CreateDelegate(method, out _lpfnDisplayCancelableProgressBar);
            }

            var result = _lpfnDisplayCancelableProgressBar(title, info, progress);
            return result;
        }

        private static Action _lpfnClearProgressBar;
        private static void _ClearProgressBar ()
        {
            if (!_IsEditor())
            {
                return;
            }

            if (null == _lpfnClearProgressBar)
            {
                var type = System.Type.GetType("UnityEditor.EditorUtility,UnityEditor");
                var method = type.GetMethod("ClearProgressBar", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                TypeTools.CreateDelegate(method, out _lpfnClearProgressBar);
            }

            _lpfnClearProgressBar();
        }

        private static bool _IsEditor ()
        {
            return true;
        }
    }
}
