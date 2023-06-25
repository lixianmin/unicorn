
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
        public static void Dispose (this IDisposable disposable)
        {
            disposable?.Dispose();
        }
    }
}