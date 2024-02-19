
/********************************************************************
created:    2015-10-20
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ExtendedIList
    {
        public static int GetCount (this IList list)
        {
            if (null != list)
            {
                var count = list.Count;
                return count;
            }

            return 0;
        }
        
        public static int MoveTo (this IList srcList, IList destList)
        {
			if (null != srcList && null != destList)
			{
				var count = srcList.Count;
				for (int i= 0; i< count; ++i)
				{
					var item = srcList[i];
					destList.Add(item);
				}

				srcList.Clear();
				return count;
			}

			return 0;
		}
        
        public static void Reserve<T> (this List<T> list, int minCapacity)
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

        //       public static int MoveTo (this IList srcList, IList destList, object locker)
		// {
		// 	if (null != srcList && null != destList && null != locker)
		// 	{
		// 		lock (locker)
		// 		{
		// 			var count = srcList.Count;
		// 			if (count > 0)
		// 			{
		// 				for (var i= 0; i< count; ++i)
		// 				{
		// 					var item = srcList[i];
		// 					destList.Add(item);
		// 				}
		// 			
		// 				srcList.Clear();
		// 			}
		// 			
		// 			return count;
		// 		}
		// 	}
		//
		// 	return 0;
		// }

		public static Type GetElementType (this IList list)
		{
			if (null != list)
			{
				var listType = list.GetType();

				if (list is Array)
				{
					var elementType = listType.GetElementType();
					return elementType;
				}

				if (listType.IsGenericType)
				{
					var genericTypeDefinition = listType.GetGenericTypeDefinition();
					if (typeof(List<>) == genericTypeDefinition)
					{
						var elementType = listType.GetGenericArguments() [0];
						return elementType;
					}
				}
			}

			return null;
		}

        public static void EnsureSize<T> (this IList<T> list, int size)
        {
            if (null != list)
            {
                var count = list.Count;
                for (int i= count; i< size; ++i)
                {
                    list.Add(default);
                }
            }
        }

        public static void Clear<T> (this IList<T> list)
        {
            if (list is { Count: > 0 })
            {
                list.Clear();
            }
        }
        
        public static bool IsNullOrEmpty<T> (this List<T> list)
        {
	        return list is { Count: 0 };
        }

        public static bool AddIfNotNull<T> (this IList<T> list, T item)
        {
            if (null != list && null != item)
            {
	            list.Add(item);
	            return true;
            }

            return false;
        }
        
        public static bool AddUnique<T> (this IList<T> list, T item)
        {
	        if (null != list && list.IndexOf(item) < 0)
	        {
		        list.Add(item);
		        return true;
	        }

	        return false;
        }
        
        public static T PopBack<T> (this IList<T> list)
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
            
	        return default;
        }
        
        public static T Back<T> (this IList<T> list)
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
            
	        return default;
        }
        
        public static void DisposeAllAndClear<T>(this IList<T> list)
        {
	        var count = list?.Count;
	        if (count > 0)
	        {
		        for (int i = 0; i < count; i++)
		        {
			        var item = list[i] as IDisposable;
			        item?.Dispose();
		        }

		        list.Clear();
	        }
        }
    }
}