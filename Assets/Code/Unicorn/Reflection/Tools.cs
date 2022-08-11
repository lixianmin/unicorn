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
//	public enum Tool
//	{
//		View,
//		Move,
//		Rotate,
//		Scale,
//		None = -1
//	}
//
//	public static class Tools
//	{
//		private static System.Reflection.PropertyInfo _current;
//
//		public static Tool current
//		{
//			get
//			{
//				if (null == _current)
//				{
//					_current = MyType.GetProperty("current");
//				}
//
//				return (Tool) _current.GetValue(null, null);
//			}
//
//			set
//			{
//				if (null == _current)
//				{
//					_current = MyType.GetProperty("current");
//				}
//
//				_current.SetValue(null, value, null);
//			}
//		}
//		private static Type _myType;
//
//		public static Type MyType
//		{
//			get
//			{
//				if (null == _myType)
//				{
//					_myType = System.Type.GetType("UnityEditor.Tools,UnityEditor");
//				}
//
//				return _myType;
//			}
//		}
//	}
//}
