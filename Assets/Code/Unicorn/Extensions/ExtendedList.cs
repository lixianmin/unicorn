
/********************************************************************
created:	2014-10-25
author:		lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static class ExtendedList
    {
        public static void ReserveEx<T> (this List<T> list, int minCapacity)
        {
            if (null != list)
            {
                var capacity = list.Capacity;
                
                if (minCapacity > capacity)
                {
                    list.Capacity = Math.Max (Math.Max (capacity * 2, 4), minCapacity);
                }
            }
        }
        
        public static T PopBackEx<T> (this List<T> list)
        {
            if (null != list)
            {
                var count = list.Count;
                if (count > 0)
                {
                    var idxLast = count - 1;
                    var back = list[idxLast];
                    list.RemoveAt(idxLast);
                    
                    return back;
                }
            }
            
            return default(T);
        }
        
        public static T BackEx<T> (this List<T> list)
        {
            if (null != list)
            {
                var count = list.Count;
                if (count > 0)
                {
                    var idxLast = count - 1;
                    var back = list[idxLast];
                    
                    return back;
                }
            }
            
            return default(T);
        }
        
        public static bool EmptyEx<T> (this List<T> list)
        {
            return null != list && list.Count == 0;
        }

        public static bool AddUnicornEx<T> (this List<T> list, T item)
        {
            if (null != list && list.IndexOf(item) < 0)
            {
                list.Add(item);
                return true;
            }

            return false;
        }
    }

}