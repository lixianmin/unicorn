/********************************************************************
created:    2022-08-31
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ExtendedListAction
    {   
	    /// <summary>
	    /// 用于实现RemoveAllListeners()的unregister事件回调函数的效果
	    /// </summary>
	    /// <param name="list"></param>
        public static void InvokeAndClearEx (this IList<Action> list)
		{
			if (null != list)
			{
				var count = list.Count;
				if (count > 0)
				{
					for (var i = 0; i < count; i++)
					{
						var action = list[i];
						action?.Invoke();
					}
					
					list.Clear();
				}
			}
		}
    }
}