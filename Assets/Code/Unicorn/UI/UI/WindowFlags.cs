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
        Cache = 0x01, // 加载后, 缓存窗体在内存中, 不执行Unload
    }
}