
/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public interface IWebNode
    {
        // Task使用IsCompleted，而AsyncOperationHandle使用IsDone，先保持IsDone吧
        /// <summary>
        /// 下载过程是否已结束：成功/失败/取消
        /// </summary>
        bool IsDone    { get; } 
        
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