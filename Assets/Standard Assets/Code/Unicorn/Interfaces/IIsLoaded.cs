/*********************************************************************
created:    2025-04-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    /// <summary>
    /// 协程相关的代码, 经常需要判断当前的gameObject是否还存在, 以便决定是否继续执行
    /// IHaveTransform代替不了IIsLoaded, 比如UIWindowBase就是以标记flag来判断IsLoaded()的
    /// </summary>
    public interface IIsLoaded
    {
        bool IsLoaded();
    }
}