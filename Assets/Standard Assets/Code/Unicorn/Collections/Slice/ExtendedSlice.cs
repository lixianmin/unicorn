﻿/********************************************************************
created:    2023-12-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Collections;

namespace Unicorn
{
    public static class ExtendedSlice
    {
        public static bool IsNullOrEmpty<T>(this Slice<T> my)
        {
            return my == null || my.Size == 0;
        }
    }
}