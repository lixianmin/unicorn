
/********************************************************************
created:    2015-04-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn
{
    public static class ExtendedIDisposable
    { 
        public static void DisposeEx (this IDisposable disposable)
        {
            if (null != disposable)
			{
				disposable.Dispose();
			}
        }
    }
}