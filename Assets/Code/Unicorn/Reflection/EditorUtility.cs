
/********************************************************************
created:    2015-03-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System;

using Object = UnityEngine.Object;

namespace Unicorn.Reflection
{
	public static class EditorUtility
	{
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void ClearProgressBar ()
		{
			if (!os.isEditor)
			{
				return;
			}

			_CheckCreateStaticDelegate("ClearProgressBar", ref _lpfnClearProgressBar);
			_lpfnClearProgressBar();
		}

		public static Object[] CollectDeepHierarchy (Object[] roots)
		{
			if (!os.isEditor)
			{
				return Array.Empty<Object>();
			}

			_CheckCreateStaticDelegate("CollectDeepHierarchy", ref _lpfnCollectDeepHierarchy);
			var result = _lpfnCollectDeepHierarchy(roots);
			return result;
		}

		public static Object[] CollectDependencies (Object[] roots)
		{
			if (!os.isEditor)
			{
				return Array.Empty<Object>();
			}

			_CheckCreateStaticDelegate("CollectDependencies", ref _lpfnCollectDependencies);
			var result = _lpfnCollectDependencies(roots);
			return result;
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void CopySerialized (Object source, Object dest)
		{
			if (!os.isEditor)
			{
				return;
			}

			_CheckCreateStaticDelegate("CopySerialized", ref _lpfnCopySerialized);
			_lpfnCopySerialized(source, dest);
		}

		public static GameObject CreateGameObjectWithHideFlags (string name, HideFlags flags, params Type[] components)
		{
			if (!os.isEditor)
			{
				return null;
			}

			_CheckCreateStaticDelegate("CreateGameObjectWithHideFlags", ref _lpfnCreateGameObjectWithHideFlags);
			var result = _lpfnCreateGameObjectWithHideFlags(name, flags, components);
			return result;
		}

		public static bool DisplayDialog (string title, string message, string ok, string cancel = "")
		{
			if (!os.isEditor)
			{
				return false;
			}

            if (null == _lpfnDisplayDialog)
            {
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
                var method = MyType.GetMethod("DisplayDialog", flags, null, new Type[]{ typeof(string), typeof(string), typeof(string), typeof(string) }, null);
                TypeTools.CreateDelegate(method, out _lpfnDisplayDialog);
            }

			var result = _lpfnDisplayDialog(title, message, ok, cancel);
			return result;
		}

		public static bool DisplayCancelableProgressBar (string title, string info, float progress)
		{
			if (!os.isEditor)
			{
				return false;
			}

			_CheckCreateStaticDelegate("DisplayCancelableProgressBar", ref _lpfnDisplayCancelableProgressBar);
			var result = _lpfnDisplayCancelableProgressBar(title, info, progress);
			return result;
		}

		public static Object InstanceIDToObject (int instanceID)
		{
			if (!os.isEditor)
			{
				return null;
			}

			_CheckCreateStaticDelegate("InstanceIDToObject", ref _lpfnInstanceIDToObject);
			var result = _lpfnInstanceIDToObject(instanceID);
			return result;
		}

		public static bool IsPersistent (Object target)
		{
			if (!os.isEditor)
			{
				return false;
			}

			_CheckCreateStaticDelegate("IsPersistent", ref _lpfnIsPersistent);
			var result = _lpfnIsPersistent(target);
			return result;
		}

		public static string OpenFilePanel (string title, string directory, string extension)
		{
			if (!os.isEditor)
			{
				return string.Empty;
			}

			_CheckCreateStaticDelegate("OpenFilePanel", ref _lpfnOpenFilePanel);
			var result = _lpfnOpenFilePanel(title, directory, extension);
			return result;
		}

		public static string OpenFolderPanel (string title, string folder, string defaultName)
		{
			if (!os.isEditor)
			{
				return string.Empty;
			}

			_CheckCreateStaticDelegate("OpenFolderPanel", ref _lpfnOpenFolderPanel);
			var result = _lpfnOpenFolderPanel(title, folder, defaultName);
			return result;
		}

		public static string SaveFilePanel (string title, string directory, string defaultName, string extension)
		{
			if (!os.isEditor)
			{
				return string.Empty;
			}

			_CheckCreateStaticDelegate("SaveFilePanel", ref _lpfnSaveFilePanel);
			var result = _lpfnSaveFilePanel(title, directory, defaultName, extension);
			return result;
		}

		public static string SaveFilePanelInProject (string title, string defaultName, string extension, string message)
		{
			if (!os.isEditor)
			{
				return string.Empty;
			}

			_CheckCreateStaticDelegate("SaveFilePanelInProject", ref _lpfnSaveFilePanelInProject);
			var result = _lpfnSaveFilePanelInProject(title, defaultName, extension, message);
			return result;
		}

		public static string SaveFolderPanel (string title, string folder, string defaultName)
		{
			_CheckCreateStaticDelegate("SaveFolderPanel", ref _lpfnSaveFolderPanel);
			var result = SaveFolderPanel(title, folder, defaultName);
			return result;
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void SetDirty (Object target)
		{
			if (!os.isEditor)
			{
				return;
			}

			_CheckCreateStaticDelegate("SetDirty", ref _lpfnSetDirty);
			_lpfnSetDirty(target);
		}

		private static void _CheckCreateStaticDelegate<T> (string name, ref T lpfnMethod) where T : class
		{
			if (null == lpfnMethod)
			{
				var method = MyType.GetMethod(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				TypeTools.CreateDelegate(method, out lpfnMethod);
			}
		}

		private static Type _myType;

		public static Type MyType
		{
			get
			{
				if (null == _myType && os.isEditor)
				{
					_myType = System.Type.GetType("UnityEditor.EditorUtility,UnityEditor");
				}

				return _myType;
			}
		}

		private static Action _lpfnClearProgressBar;

		private static Func<Object[], Object[]> _lpfnCollectDeepHierarchy;

		private static Func<Object[], Object[]> _lpfnCollectDependencies;

		private static Action<Object, Object>	_lpfnCopySerialized;

		private static Func<string, HideFlags, Type[], GameObject> _lpfnCreateGameObjectWithHideFlags;

		private static Func<string, string, string, string, bool> _lpfnDisplayDialog;

		private static Func<string, string, float, bool> _lpfnDisplayCancelableProgressBar;

		private static Func<int, Object> _lpfnInstanceIDToObject;

		private static Func<Object, bool> _lpfnIsPersistent;

		private static Func<string, string, string, string> _lpfnOpenFilePanel;

		private static Func<string, string, string, string> _lpfnOpenFolderPanel;

		private static Func<string, string, string, string, string> _lpfnSaveFilePanel;

		private static Func<string, string, string, string, string> _lpfnSaveFilePanelInProject;

		private static Func<string, string, string, string> _lpfnSaveFolderPanel;

		private static Action<Object> _lpfnSetDirty;
	}
}
