/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Text;

namespace Unicorn
{
    public static class StringBuilderPool
    {
        /// <summary>
        /// 这个类解决了几个很关键的问题：
        /// 1. 线程安全： 不同的线程，会获取到不同的StringBuilder对象
        /// 2. 允许忘记归还： 把_cache取走后，_cache会一直是null，直到有人记得还上为止
        /// 3. 每次Get时候cache.Length重置为0，这样用户拿到的一定是重置过的
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static StringBuilder Get(int capacity = 256)
        {
            var cache = _cache;
            if (cache != null && cache.Capacity >= capacity)
            {
                _cache = null;
                cache.Length = 0;
                return cache;
            }

            return new StringBuilder(capacity);
        }

        public static string GetStringAndReturn(StringBuilder sb)
        {
            if (null != sb)
            {
                var text = sb.ToString();
                Return(sb);
                return text;
            }

            return string.Empty;
        }

        public static void Return(StringBuilder sb)
        {
            const int maxBuilderSize = 1024;
            if (sb is { Capacity: <= maxBuilderSize })
            {
                _cache = sb;
            }
        }

        [ThreadStatic] private static StringBuilder _cache;
    }
}