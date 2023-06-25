
/*********************************************************************
created:    2014-03-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ExtendedArray
    {
        internal static T[] BlockClone<T> (this T[] array) where T : struct
        {
            if (null != array)
            {
                int length = array.Length;
                var cloned = new T[length];
                Buffer.BlockCopy(array, 0, cloned, 0, Buffer.ByteLength(array));
                return cloned;
            }

            return null;
        }

        public static void Clear<T> (this T[] array)
        {
            if (null != array)
            {
                Array.Clear(array, 0, array.Length);
            }
        }

		public static T[] Append<T> (this T[] array, T a)
		{
			var oldSize = null == array ? 0 : array.Length;
            Array.Resize(ref array, oldSize + 1);
            array[oldSize] = a;

            return array;
		}

        public static T[] Append<T> (this T[] array, IEnumerable<T> list)
        {
            if(null != list)
            {
                var appendLength    = System.Linq.Enumerable.Count(list);
                var orginalLength   = null == array ? 0 : array.Length;
                var next = new T[orginalLength + appendLength];

                if (null != array && orginalLength > 0)
                {
                    Array.Copy(array, next, orginalLength);
                }

                var e = list.GetEnumerator();
                using (e)
                {
                    var index = orginalLength;
                    while (e.MoveNext())
                    {
                        next[index++] = e.Current;
                    }
                }

                return next;
            }

            return array;
        }

        public static bool IsNullOrEmpty<T> (this T[] array)
        {
            return null == array || array.Length == 0;
        }

		public static int GetTotalLength (this Array array)
		{
			if (null == array)
			{
				return 0;
			}

			var rank = array.Rank;
			if (rank == 1)
			{
				return array.Length;
			}
			
			var length = 1;
			while (rank > 0)
			{
				length *= array.GetLength( rank - 1 );
				rank--;
			}
			
			return length;
		}
    }
}