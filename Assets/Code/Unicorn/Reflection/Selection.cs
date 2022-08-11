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
//	public static class Selection
//	{
//		private static System.Reflection.PropertyInfo _activeGameObject;
//
//		public static GameObject activeGameObject
//		{
//			get
//			{
//				if (null == _activeGameObject)
//				{
//					_activeGameObject = MyType.GetProperty("activeGameObject");
//				}
//
//				return (GameObject)_activeGameObject.GetValue(null, null);
//			}
//			set
//			{
//				if (null == _activeGameObject)
//				{
//					_activeGameObject = MyType.GetProperty("activeGameObject");
//				}
//				
//				_activeGameObject.SetValue(null, value, null);
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
//					_myType = System.Type.GetType("UnityEditor.Selection,UnityEditor");
//				}
//
//				return _myType;
//			}
//		}
//
//	}
//}
