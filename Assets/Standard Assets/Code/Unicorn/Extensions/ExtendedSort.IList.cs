/*********************************************************************
created:    2014-01-06
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    static partial class ExtendedSort
    {
        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _QuickSort(list, 0, list.Count, comparison);
        }

        public static void Sort<T>(this IList<T> list, int index, int length, Comparison<T> comparison)
        {
            _CheckArguments(list, comparison);
            _ClampIndexAndLength(ref index, ref length, list.Count);
            _QuickSort(list, index, length, comparison);
        }

        private static void _QuickSort<T>(IList<T> list, int index, int length, Comparison<T> comparison)
        {
            const int insertionSortThreshold = 10; // Adjust this threshold as needed
            while (length > 1)
            {
                if (length <= insertionSortThreshold)
                {
                    // Use insertion sort for small arrays
                    _InsertSort(list, index, length, comparison);
                    break;
                }
                
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
                }
                else
                {
                    break;
                }
            }
        }

        // public static void InsertSort<T>(this IList<T> list, Comparison<T> comparison)
        // {
        //     _CheckArguments(list, comparison);
        //     _InsertSort(list, 0, list.Count, comparison);
        // }
        //
        // public static void InsertSort<T>(this IList<T> list, int index, int length, Comparison<T> comparison)
        // {
        //     _CheckArguments(list, comparison);
        //     _ClampIndexAndLength(ref index, ref length, list.Count);
        //     _InsertSort(list, index, length, comparison);
        // }

        private static void _InsertSort<T>(IList<T> list, int index, int length, Comparison<T> comparison)
        {
            var end = index + length;
            for (var i = index + 1; i < end; i++)
            {
                var key = list[i];
                var j = i - 1;
                while (j >= index && comparison(list[j], key) > 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }

                list[j + 1] = key;
            }
        }
    }
}