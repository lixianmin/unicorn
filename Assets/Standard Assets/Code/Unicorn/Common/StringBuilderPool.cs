
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
        public static StringBuilder Spawn(int capacity = 256)
        {
            var cache = StringBuilderPool._cache;
            if (cache != null && cache.Capacity >= capacity)
            {
                _cache = null;
                cache.Length = 0;
                return cache;
            }

            return new StringBuilder(capacity);
        }

        public static string GetStringAndRecycle(StringBuilder sb)
        {
            if (null != sb)
            {
                string text = sb.ToString();
                Recycle(sb);
                return text;
            }

            return string.Empty;
        }

        public static void Recycle(StringBuilder sb)
        {
            if (null != sb && sb.Capacity <= MAX_BUILDER_SIZE)
            {
                _cache = sb;
            }
        }

        [ThreadStatic]
        private static StringBuilder _cache = new StringBuilder(256);

        private const int MAX_BUILDER_SIZE = 512;
    }
}
