/*********************************************************************
created:    2014-01-06
author:     lixianmin

 1. Array.Sort(), 每帧有 gc alloc
 2. Array.Sort(), 带int index, int length参数的时候, 不支持Comparison<T>参数

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public static partial class ExtendedSort
    {
        public static void Sort<T>(this T[] list, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _QuickSort(list, 0, list.Length, comparison);
        }

        public static void Sort<T>(this T[] list, int index, int length, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _ClampIndexAndLength(ref index, ref length, list.Length);
            _QuickSort(list, index, length, comparison);
        }

        private static void _QuickSort<T>(T[] list, int index, int length, Comparison<T> comparison)
        {
            while (length > 1)
            {
                var pivot = list[index + length / 2];
                var i = index;
                var j = index + length - 1;

                while (i <= j)
                {
                    while (comparison(list[i], pivot) < 0)
                    {
                        ++i;
                    }

                    while (comparison(list[j], pivot) > 0)
                    {
                        --j;
                    }

                    if (i <= j)
                    {
                        (list[i], list[j]) = (list[j], list[i]);
                        ++i;
                        --j;
                    }
                }

                if (j > index)
                {
                    _QuickSort(list, index, j - index + 1, comparison);
                }

                if (i < index + length - 1)
                {
                    var index1 = index;
                    index = i;
                    length = index1 + length - i;
                    continue;
                }

                break;
            }
        }

        public static void InsertSort<T>(this T[] list, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _InsertSort(list, 0, list.Length, comparison);
        }

        public static void InsertSort<T>(this T[] list, int index, int length, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _ClampIndexAndLength(ref index, ref length, list.Length);
            _InsertSort(list, index, length, comparison);
        }

        private static void _InsertSort<T>(T[] list, int index, int length, Comparison<T> comparison)
        {
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

        private static void _CheckArguments<T>(object list, Comparison<T> comparison)
        {
            if (null == list)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (null == comparison)
            {
                throw new ArgumentNullException(nameof(comparison));
            }
        }

        private static void _ClampIndexAndLength(ref int index, ref int length, int totalSize)
        {
            index = Math.Clamp(index, 0, totalSize);
            length = Math.Clamp(length, 0, totalSize - index);
        }
    }
}