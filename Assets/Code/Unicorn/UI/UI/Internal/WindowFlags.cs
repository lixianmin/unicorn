/********************************************************************
created:    2024-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.UI.Internal
{
    [Flags]
    internal enum WindowFlags : byte
    {
        None = 0x00,
        WindowCached = 0x01,
        Loaded = 0x02,
        Opened = 0x04,
    }
}