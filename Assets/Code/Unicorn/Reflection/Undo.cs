//
///********************************************************************
//created:    2015-03-03
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//using System;
//using Object = UnityEngine.Object;
//
//namespace Unicorn.Reflection
//{
//	public static class Undo
//	{
//		[System.Diagnostics.Conditional("UNITY_EDITOR")]
//		public static void RecordObject (Object objectToUndo, string name)
//		{
//			_CheckCreateStaticDelegate("RecordObject", ref _lpfnRecordObject);
//			_lpfnRecordObject(objectToUndo, name);
//		}
//
//		[System.Diagnostics.Conditional("UNITY_EDITOR")]
//		public static void RecordObjects (Object[] objectsToUndo, string name)
//		{
//			_CheckCreateStaticDelegate("RecordObjects", ref _lpfnRecordObjects);
//			_lpfnRecordObjects(objectsToUndo, name);
//		}
//
//		[System.Diagnostics.Conditional("UNITY_EDITOR")]
//		public static void RegisterCreatedObjectUndo (Object objectToUndo, string name)
//		{
//			_CheckCreateStaticDelegate("RegisterCreatedObjectUndo", ref _lpfnRegisterCreatedObjectUndo);
//			_lpfnRegisterCreatedObjectUndo(objectToUndo, name);
//		}
//
//		[System.Diagnostics.Conditional("UNITY_EDITOR")]
//		public static void PerformUndo ()
//		{
//			_CheckCreateStaticDelegate("PerformUndo", ref _lpfnPerformUndo);
//			_lpfnPerformUndo();
//		}
//
//		[System.Diagnostics.Conditional("UNITY_EDITOR")]
//		public static void DestroyObjectImmediate (Object objectToUndo)
//		{
//			_CheckCreateStaticDelegate("DestroyObjectImmediate", ref _lpfnDestroyObjectImmediate);
//			_lpfnDestroyObjectImmediate(objectToUndo);
//		}
//
//		private static void _CheckCreateStaticDelegate<T> (string name, ref T lpfnMethod) where T : class
//		{
//			if (null == lpfnMethod)
//			{
//				var method = MyType.GetMethod(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
//				TypeTools.CreateDelegate(method, out lpfnMethod);
//			}
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
//					_myType = System.Type.GetType("UnityEditor.Undo,UnityEditor");
//				}
//
//				return _myType;
//			}
//		}
//
//		private static Action<Object, string> _lpfnRecordObject;
//
//		private static Action<Object[], string> _lpfnRecordObjects;
//
//		private static Action<Object, string> _lpfnRegisterCreatedObjectUndo;
//
//		private static Action _lpfnPerformUndo;
//
//		private static Action<Object> _lpfnDestroyObjectImmediate;
//	}
//}
