
/*********************************************************************
created:    2012-07-30
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/
using System;
using System.Collections.Generic;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedBinarySearch
    {
        public static int BinarySearchEx<T> (this IList<T> list, int key, Func<T, int> extract)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list is null");
            }

            int count = list.Count;
            int i = -1;
            int j = count;
            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (extract(list [mid]) < key)
                {
                    i = mid;
                }
				else
                {
                    j = mid;
                }
            }

            if (j == count || extract(list [j]) != key)
            {
                j = ~j;
            }

            return j;
        }

        public static int BinarySearchEx<T> (this IList<T> list, float key, Func<T, float> extract)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list is null");
            }

            int count = list.Count;
            int i = -1;
            int j = count;
            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (extract(list [mid]) < key)
                {
                    i = mid;
                }
				else
                {
                    j = mid;
                }
            }

            if (j == count || !os.isEqual(extract(list [j]), key))
            {
                j = ~j;
            }

            return j;
        }

        public static int BinarySearchEx<T> (this T[] list, int key, Func<T, int> extract)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list is null");
            }

            int count = list.Length;
            int i = -1;
            int j = count;
            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (extract(list [mid]) < key)
                {
                    i = mid;
                }
				else
                {
                    j = mid;
                }
            }

            if (j == count || extract(list [j]) != key)
            {
                j = ~j;
            }

            return j;
        }

        public static int BinarySearchEx<T> (this T[] list, float key, Func<T, float> extract)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list is null");
            }

            int count = list.Length;
            int i = -1;
            int j = count;
            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (extract(list [mid]) < key)
                {
                    i = mid;
                }
				else
                {
                    j = mid;
                }
            }

            if (j == count || !os.isEqual(extract(list [j]), key))
            {
                j = ~j;
            }

            return j;
        }

        public static int BinarySearchEx<ItemType, KeyType> (this ItemType[] list, KeyType key, Func<ItemType, KeyType> extract)
            where KeyType: IEquatable<KeyType>, IComparable<KeyType>
        {
            if (null == list)
            {
                throw new ArgumentNullException("list is null");
            }
            
            int count = list.Length;
            int i = -1;
            int j = count;
            while (i + 1 != j)
            {
                int mid = i + (j - i >> 1);
                if (extract(list [mid]).CompareTo(key) < 0)
                {
                    i = mid;
                }
				else
                {
                    j = mid;
                }
            }
            
            if (j == count || !extract(list [j]).Equals(key))
            {
                j = ~j;
            }
            
            return j;
        }
    }
}