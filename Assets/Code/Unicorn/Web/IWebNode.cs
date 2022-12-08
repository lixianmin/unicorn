
/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public interface IWebNode
    {
        /// <summary>
        /// 下载过程是否已结束：成功/失败/取消。只所以使用IsCompleted代替IsDone是因为Task使用IsCompleted
        /// </summary>
        bool IsCompleted    { get; } 
        
        /// <summary>
        /// 是否下载成功
        /// </summary>
        bool IsSucceeded    { get; }
        
        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object Asset { get; }
    }
}