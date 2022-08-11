//
///********************************************************************
//created:    2015-03-03
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//using System;
//
//namespace Unicorn.Reflection
//{
//	public static class EditorPrefs
//	{
//		public static int GetInt (string key, int defaultValue = 0)
//		{
//			if (null == _lpfnGetInt)
//			{
//				var method = MyType.GetMethod("GetInt", new Type[]{typeof(string), typeof(int)});
//				TypeTools.CreateDelegate(method, out _lpfnGetInt);
//			}
//
//			var result = _lpfnGetInt(key, defaultValue);
//			return result;
//		}
//
//		public static void SetInt (string key,int value)
//		{
//			if (null == _lpfnSetInt)
//			{
//				var method = MyType.GetMethod("SetInt", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
//				TypeTools.CreateDelegate(method, out _lpfnSetInt);
//			}
//
//			_lpfnSetInt(key, value);
//		}
//
//		private static Type _myType;
//
//		public static Type MyType
//		{
//			get
//			{
//				if (null == _myType)
//				{
//					_myType = System.Type.GetType("UnityEditor.EditorPrefs,UnityEditor");
//				}
//
//				return _myType;
//			}
//		}
//
//		private static Func<string, int, int> _lpfnGetInt;
//		private static Action<string, int> _lpfnSetInt;
//	}
//}
