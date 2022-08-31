
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
        public static int GetCountEx (this IList list)
        {
            if (null != list)
            {
                var count = list.Count;
                return count;
            }

            return 0;
        }

        public static int MoveToEx (this IList srcList, IList destList)
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

        //       public static int MoveToEx (this IList srcList, IList destList, object locker)
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

		public static Type GetElementTypeEx (this IList list)
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

        public static void EnsureSizeEx<T> (this IList<T> list, int size)
        {
            if (null != list)
            {
                var count = list.Count;
                for (int i= count; i< size; ++i)
                {
                    list.Add(default(T));
                }
            }
        }

        public static void ClearEx<T> (this IList<T> list)
        {
            if (null != list && list.Count > 0)
            {
                list.Clear();
            }
        }

//        public static void AddIfNotNullEx<T> (this IList<T> list, T item)
//        {
//            if (null != list && null != item)
//            {
//                list.Add(item);
//            }
//        }
    }
}