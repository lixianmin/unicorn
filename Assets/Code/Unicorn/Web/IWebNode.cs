﻿/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public interface IWebNode
    {
        WebStatus Status { get; }

        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object Asset { get; }
    }
}