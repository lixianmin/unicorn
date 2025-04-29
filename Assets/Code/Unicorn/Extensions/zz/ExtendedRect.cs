//
///********************************************************************
//created:	2011-12-09
//author:		lixianmin
//
//purpose:	Extensions for Rect
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using UnityEngine;
//
//namespace Unicorn
//{
//    
//    internal static class ExtendedRect
//    {
//        public static bool IsInScreenEx (this Rect area)
//        {
//            return area.xMax >= 0 && area.yMax >= 0 && area.xMin < Screen.width && area.yMin < Screen.height;
//        }
//        
//        public static bool IsIntersectEx (this Rect area, Rect other)
//        {
//            return !(other.xMax < area.x || other.yMax < area.y || other.x > area.xMax || other.y > area.yMax);
//        }
//    }
//}