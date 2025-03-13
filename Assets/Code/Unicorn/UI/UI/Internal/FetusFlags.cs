/********************************************************************
created:    2024-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.UI.Internal
{
    [Flags]
    internal enum FetusFlags : byte
    {
        None = 0x00,
        Cache = 0x01,

        Loaded = 0x10,
        Opened = 0x20, // 指OpenedState
        // Disposed = 0x40, // 指使用CloseWindow()关闭了这个窗体
    }
}