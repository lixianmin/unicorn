
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using Unicorn.Reflection;

namespace Unicorn
{
	public static partial class os
	{
		static os()
		{
			isEditor = Application.isEditor;

			var platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer)
			{
				isIPhonePlayer = true;
			}
			else if (platform == RuntimePlatform.Android)
			{
				isAndroid = true;
			}

			isBigMemory = SystemInfo.systemMemorySize > 1024 + 512;
			isWindows = platform == RuntimePlatform.WindowsEditor
						|| platform == RuntimePlatform.WindowsPlayer;

			frameCount = Time.frameCount;
			time = Time.realtimeSinceStartup;

			// init editor mode properties.
			_InitModeTypes();
		}

		public static void startfile(string filename, string arguments = null, bool shell = false)
		{
			Kernel.startfile(filename, arguments, shell);
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
			Kernel.makedirs(name);
		}

		public static string[] walk(string path, string searchPattern)
		{
			return Kernel.walk(path, searchPattern);
		}

		public static void dispose<T>(ref T obj) where T : class, System.IDisposable
		{
			if (null != obj)
			{
				obj.Dispose();
				obj = null;
			}
		}

		//		public static void recycle (System.IDisposable obj)
		//		{
		//			DisposableRecycler.Recycle(obj);
		//		}

		public static bool isEqual(float a, float b)
		{
			const float eps = 0.000001f;

			var delta = a - b;
			return delta < eps && delta > -eps;
		}

		public static void swap<T>(ref T lhs, ref T rhs)
		{
			var temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		public static void collectgarbage()
		{
			Unicorn.Collections.WeakTable.CollectGarbage();
			//WeakCacheManager.Instance.Clear();
			//PrefabPool.ClearPools();

			//if (!isBigMemoryMode)
			//{
			//	var prefabCache = Unicorn.Web.WebPrefab.GetLruCache();
			//	prefabCache.TrimExcess();

			//	var webCache = Unicorn.Web.WebItem.GetLruCache();
			//	webCache.TrimExcess();
			//}

			// called in game client, for flexibility.
			//	Resources.UnloadUnusedAssets();
			//	GC.Collect();
		}

		public static AsyncOperation UnloadUnusedAssets()
		{
			isUnloadingUnusedAssets = true;
			var ao = Resources.UnloadUnusedAssets();
			CoroutineManager.Instance.StartCoroutine(_CoUnloadUnusedAssets(ao));

			return ao;
		}

		private static IEnumerator _CoUnloadUnusedAssets(AsyncOperation ao)
		{
			while (!ao.isDone)
			{
				yield return null;
			}

			isUnloadingUnusedAssets = false;
		}

		private static Action<object, string> _lpfnWatchObject;
		public static void WatchObject(object target, string title = null)
		{
			if (null == target)
			{
				return;
			}

			if (null == _lpfnWatchObject)
			{
				var type = TypeTools.SearchType("Unicorn.Diagnostics.HeapDumpManager");
				var method = type.GetMethod("WatchObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				TypeTools.CreateDelegate(method, out _lpfnWatchObject);
			}

			if (null != _lpfnWatchObject)
			{
				_lpfnWatchObject(target, title);
			}
		}

		private static bool _isTargetPlatformChecked;
		private static TargetPlatform _targetPlatform;

		public static TargetPlatform targetPlatform
		{
			get
			{
				if (!_isTargetPlatformChecked)
				{
					_isTargetPlatformChecked = true;

					if (Application.platform == RuntimePlatform.Android)
					{
						_targetPlatform = TargetPlatform.Android;
					}
					else if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						_targetPlatform = TargetPlatform.iPhone;
					}
					// else if (Application.isWebPlayer)
					// {
					//     _targetPlatform = TargetPlatform.WebPlayer;
					// }
					else if (Application.isEditor)
					{
						_targetPlatform = EditorUserBuildSettings.activeBuildTarget;
					}
				}

				return _targetPlatform;
			}
		}

		public static bool isEditor { get; private set; }
		public static bool isIPhonePlayer { get; private set; }
		public static bool isAndroid { get; private set; }
		public static bool isBigMemory { get; private set; }
		public static bool isWindows { get; private set; }
		internal static bool isUnloadingUnusedAssets { get; private set; }

		public static int frameCount { get; internal set; }
		public static float time { get; internal set; }

		public const string linesep = "\n";

		public static readonly StringIntern intern = new StringIntern();
		public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
	}
}