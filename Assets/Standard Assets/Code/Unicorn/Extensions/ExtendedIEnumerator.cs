
/********************************************************************
created:    2016-04-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn
{
    internal static class ExtendedIEnumerator
    {   
        public static void MoveEndEx (this IEnumerator iter)
		{
			if (null != iter)
			{
				while (iter.MoveNext())
				{
				}
			}
		}
    }
}