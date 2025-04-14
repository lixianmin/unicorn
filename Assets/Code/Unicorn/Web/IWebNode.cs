/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public enum WebState
    {
        None,
        Loading,
        Succeeded,
        Failed,
        Canceled,
    }

    public interface IWebNode
    {
        WebState GetState();

        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object Asset { get; }
    }
}