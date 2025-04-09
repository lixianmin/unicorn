/*********************************************************************
created:    2014-03-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public static class ArrayEx
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return null == array || array.Length == 0;
        }

        public static void Clear<T>(this T[] array)
        {
            if (null != array)
            {
                Array.Clear(array, 0, array.Length);
            }
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            return array != null ? Array.IndexOf(array, item) : -1;
        }

        public static int IndexOf<T>(this T[] array, T item, int startIndex)
        {
            return array != null ? Array.IndexOf(array, item, startIndex) : -1;
        }

        public static int IndexOf<T>(this T[] array, T item, int startIndex, int count)
        {
            return array != null ? Array.IndexOf(array, item, startIndex, count) : -1;
        }

        // internal static T[] BlockClone<T>(this T[] array) where T : struct
        // {
        //     if (null != array)
        //     {
        //         var length = array.Length;
        //         var cloned = new T[length];
        //         Buffer.BlockCopy(array, 0, cloned, 0, Buffer.ByteLength(array));
        //         return cloned;
        //     }
        //
        //     return null;
        // }
        // public static T[] Append<T> (this T[] array, T a)
        // {
        // 	var oldSize = array?.Length ?? 0;
        //           Array.Resize(ref array, oldSize + 1);
        //           array[oldSize] = a;
        //
        //           return array;
        // }
        //
        //       public static T[] Append<T> (this T[] array, IEnumerable<T> list)
        //       {
        //           if(null != list)
        //           {
        //               var appendLength    = System.Linq.Enumerable.Count(list);
        //               var originalLength   = array?.Length ?? 0;
        //               var next = new T[originalLength + appendLength];
        //
        //               if (null != array && originalLength > 0)
        //               {
        //                   Array.Copy(array, next, originalLength);
        //               }
        //
        //               var e = list.GetEnumerator();
        //               using (e)
        //               {
        //                   var index = originalLength;
        //                   while (e.MoveNext())
        //                   {
        //                       next[index++] = e.Current;
        //                   }
        //               }
        //
        //               return next;
        //           }
        //
        //           return array;
        //       }
        // public static int GetTotalLength (this Array array)
        // {
        // 	if (null == array)
        // 	{
        // 		return 0;
        // 	}
        //
        // 	var rank = array.Rank;
        // 	if (rank == 1)
        // 	{
        // 		return array.Length;
        // 	}
        // 	
        // 	var length = 1;
        // 	while (rank > 0)
        // 	{
        // 		length *= array.GetLength( rank - 1 );
        // 		rank--;
        // 	}
        // 	
        // 	return length;
        // }
    }
}