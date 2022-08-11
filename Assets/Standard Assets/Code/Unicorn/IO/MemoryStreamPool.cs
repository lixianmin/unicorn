//
///********************************************************************
//created:    2017-07-27
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//using System;
//using System.IO;
//
//namespace Unicorn.IO
//{
//    public static class MemoryStreamPool
//    {
//        public static MemoryStream Spawn (int capacity = 256)
//        {
//            var cache = _cache;
//            if (cache != null && cache.Capacity >= capacity)
//            {
//                _cache = null;
//                cache.Position = 0;
//                return cache;
//            }
//
//            return new MemoryStream(capacity);
//        }
//
//        public static void Recycle (MemoryStream stream)
//        {
//            if (null != stream && stream.Capacity <= MAX_BUILDER_SIZE)
//            {
//                _cache = stream;
//            }
//        }
//
//        [ThreadStatic]
//        private static MemoryStream _cache = new MemoryStream(256);
//
//        private const int MAX_BUILDER_SIZE = 512;
//    }
//}