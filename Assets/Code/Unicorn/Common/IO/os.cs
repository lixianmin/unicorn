/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;
using UnityEngine;

namespace Unicorn
{
    public static partial class os
    {
        internal static void Init()
        {
            // 这些代码不能在MbGame这个MonoBehaviour的构造方法中调用
            // isBigMemory = SystemInfo.systemMemorySize > 1024 + 512;

            _InitIsReleaseMode();
        }

        private static void _InitIsReleaseMode()
        {
            if (Application.isEditor)
            {
                // 存储isReleaseMode的时候, 带上ProjectName, 防止同时开多个unity3d项目的时候冲突
                var key = PathTools.ProjectName + ".release.mode.enabled";
                var enabled = PlayerPrefs.GetInt(key, 0);
                IsReleaseMode = enabled == 1;
            }
            else
            {
                // 非编辑器状态, 强制为true
                IsReleaseMode = true;
            }
        }

        public static void mkdir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void makedirs(string name)
        {
            Kernel.MakeDirs(name);
        }

        public static string[] walk(string path, string searchPattern)
        {
            return Kernel.Walk(path, searchPattern);
        }

        public static void InternalSetIsReleaseMode(bool b)
        {
            IsReleaseMode = b;
        }

        // public static void swap<T>(ref T lhs, ref T rhs)
        // {
        // 	(lhs, rhs) = (rhs, lhs);
        // }

        // public static void collectgarbage()
        // {
        // 	Unicorn.Collections.WeakTable.CollectGarbage();
        // 	//WeakCacheManager.Instance.Clear();
        // 	//PrefabPool.ClearPools();
        //
        // 	//if (!isBigMemoryMode)
        // 	//{
        // 	//	var prefabCache = Unicorn.Web.WebPrefab.GetLruCache();
        // 	//	prefabCache.TrimExcess();
        //
        // 	//	var webCache = Unicorn.Web.WebItem.GetLruCache();
        // 	//	webCache.TrimExcess();
        // 	//}
        //
        // 	// called in game client, for flexibility.
        // 	//	Resources.UnloadUnusedAssets();
        // 	//	GC.Collect();
        // }

        /// <summary>
        /// 在editor中, 有时候需要模拟在真机上的运行环境, 执行release版本后的代码流程
        /// </summary>
        public static bool IsReleaseMode { get; private set; }

        // public static bool isBigMemory { get; private set; }

        public const string linesep = "\n";

        // public static readonly StringIntern intern = new();
        // public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
    }
}