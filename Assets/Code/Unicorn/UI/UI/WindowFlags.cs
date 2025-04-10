/********************************************************************
created:    2024-03-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.UI
{
    /// <summary>
    /// 与FetusFlags是同构的
    /// </summary>
    public enum WindowFlags : byte
    {
        None = 0x00,
        Cache = 0x01, // 加载后, 缓存窗体gameObject在内存中, 但会执行完整的从Loaded->Unloading的所有事件流程
    }
}