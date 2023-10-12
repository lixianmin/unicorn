/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;

namespace Unicorn
{
    public static partial class os
    {
        internal static void Init()
        {
            // 这些代码不能在MbGame这个MonoBehaviour的构造方法中调用
            // isBigMemory = SystemInfo.systemMemorySize > 1024 + 512;
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

        public static void dispose<T>(ref T obj) where T : class, System.IDisposable
        {
            if (null != obj)
            {
                obj.Dispose();
                obj = null;
            }
        }

        public static bool isEqual(float a, float b)
        {
            const float eps = 0.000001f;

            var delta = a - b;
            return delta is < eps and > -eps;
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
        public static bool isReleaseMode;

        // public static bool isBigMemory { get; private set; }
        
        public const string linesep = "\n";

        // public static readonly StringIntern intern = new();
        // public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
    }
}