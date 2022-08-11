//
///********************************************************************
//created:    2014-07-05
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//using System;
//
//namespace Unicorn.Reflection
//{
//    public static class FileUtil
//    {
//        public static string GetProjectRelativePath (string path)
//        {
//			if (null == _lpfnGetProjectRelativePath)
//            {
//                var method = MyType.GetMethod("GetProjectRelativePath", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
//				TypeTools.CreateDelegate(method, out _lpfnGetProjectRelativePath);
//			}
//
//			var result = _lpfnGetProjectRelativePath(path);
//            return result;
//        }
//
//        private static Type _myType;
//
//        public static Type MyType
//        {
//            get
//            {
//                if (null == _myType)
//                {
//                    _myType = System.Type.GetType("UnityEditor.FileUtil,UnityEditor");
//                }
//
//                return _myType;
//            }
//        }
//        
//		private static Func<string, string> _lpfnGetProjectRelativePath;
//    }
//}
