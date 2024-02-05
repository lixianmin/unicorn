/*********************************************************************
created:    2014-01-06
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ExtendedSort
    {
        public static void InsertSort<T>(this T[] list, Comparison<T> comparison)
        {
            list.InsertSort(0, list?.Length ?? 0, comparison);
        }

        public static void InsertSort<T>(this T[] list, int index, int length, Comparison<T> comparison)
        {
            if (null == list)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (null == comparison)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            _ClampIndexAndLength(ref index, ref length, list.Length);
            var end = index + length;

            for (var i = index + 1; i < end; ++i)
            {
                if (comparison(list[i], list[i - 1]) < 0)
                {
                    var temp = list[i];
                    var j = i;
                    while (j > 0 && comparison(temp, list[j - 1]) < 0)
                    {
                        list[j] = list[j - 1];
                        --j;
                    }

                    list[j] = temp;
                }
            }
        }

        private static void _ClampIndexAndLength(ref int index, ref int length, int totalSize)
        {
            index = Math.Clamp(index, 0, totalSize);
            length = Math.Clamp(length, 0, totalSize - index);
        }

        public static void InsertSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            list.InsertSort(0, list?.Count ?? 0, comparison);
        }

        public static void InsertSort<T>(this IList<T> list, int index, int length, Comparison<T> comparison)
        {
            if (null == list)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (null == comparison)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            _ClampIndexAndLength(ref index, ref length, list.Count);
            var end = index + length;

            for (var i = index + 1; i < end; ++i)
            {
                if (comparison(list[i], list[i - 1]) < 0)
                {
                    var temp = list[i];
                    var j = i;
                    while (j > 0 && comparison(temp, list[j - 1]) < 0)
                    {
                        list[j] = list[j - 1];
                        --j;
                    }

                    list[j] = temp;
                }
            }
        }
    }
}