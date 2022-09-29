
/********************************************************************
created:    2014-04-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Linq;

#if UNITY_2017_1_OR_NEWER
#pragma warning disable 0618
#endif

namespace Unicorn
{
	public static partial class EditorTools
	{
		static EditorTools()
		{
			EditorApplication.update += _OnUpdateCallback;
			// EditorSettings
			
			// 下面这个在scene打开时调用回调的方案，在头几帧的时候，收到不回调消息，猜测是编译未完成导致的
			// _AttachEditorSceneOpenedMethods();
		}

		//        private static void _AttachEditorSceneOpenedMethods ()
		//        {
		//            _sceneOpenedCallbacks = TypeTools.GetCustomAssemblies()
		//                .SelectMany(a => a.GetTypes())
		//                .SelectMany(b => b.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))
		//                .Where(c => c.GetCustomAttributes(false).OfType<EditorSceneOpenedAttribute>().Any())
		//                .ToArray();
		//
		//            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += _OnSceneOpenedCallback;
		//        }

		public static void SyncMonoProject()
		{
			EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
		}

		// for commandline usage
		public static void SwitchActiveBuildTargetToIPhone()
		{
			_SwitchActiveBuildTarget(iOSTargetTools.GetBuildTarget());
		}

		// for command line usage
		public static void SwitchActiveBuildTargetToAndroid()
		{
			_SwitchActiveBuildTarget(BuildTarget.Android);
		}

		private static void _SwitchActiveBuildTarget(BuildTarget target)
		{
			var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
			if (activeBuildTarget != target)
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(target);
			}
		}

		public static void ExecuteMenuItem()
		{
			var arg = new CustomArgument();
			string menuItemPath = arg.GetCustomArgumentString("menuItemPath");

			if (!string.IsNullOrEmpty(menuItemPath))
			{
				bool suc = UnityEditor.EditorApplication.ExecuteMenuItem(menuItemPath);
				string hint = $"Unicorn.EditorTools.ExecuteMenuItem({menuItemPath})";
				if (suc)
				{
					Console.WriteLine(hint + " Success!");
				}
				else
				{
					Console.WriteLine(hint + " Fail!");
				}
			}
			else
			{
				Console.Error.WriteLine("Unicorn.EditorApplication.ExecuteMenuItem(arg), arg is null or empty!");
			}
		}

		public static bool DisplayCancelableProgressBar(string title, string info)
		{
			const float cycleLength = 1.0f;

			var current = Time.realtimeSinceStartup;
			if (current > _nextProgressTime)
			{
				_nextProgressTime = current + cycleLength;
			}

			var progress = current + cycleLength - _nextProgressTime;
			var isCanceled = EditorUtility.DisplayCancelableProgressBar(title, info, progress);

			return isCanceled;
		}

		private static void _OnUpdateCallback()
		{
			if (!UnicornMain.Instance.IsInited)
			{
				var count = _updates.Count;
				if (count > 0)
				{
					for (int i = 0; i < count; ++i)
					{
						var callback = _updates[i] as Action;
						callback?.Invoke();
					}
				}
			}
		}

		private static void _AttachToUpdate(Action action)
		{
			if (null != action && !_updates.Contains(action))
			{
				_updates.Add(action);
			}
		}

		//        private static void _OnSceneOpenedCallback (UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
		//        {
		//            var loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
		//            if ((int) mode != 0)
		//            {
		//                loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Additive;
		//            }
		//
		//            var args = new object[]{ scene, loadSceneMode};
		//            foreach (var method in _sceneOpenedCallbacks)
		//            {
		//                method.Invoke(null, args);
		//            }
		//        }

		public static int GetScreenWidth()
		{
			var width = Screen.width;
			if (Application.isEditor && Application.platform == RuntimePlatform.OSXEditor)
			{
				return width / 2;
			}

			return width;
		}

		private static float _nextProgressTime;
		private static readonly ArrayList _updates = new ();
	}
}