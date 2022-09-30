
/********************************************************************
created:    2022-08-12
author:     lixianmin


Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public interface IWebNode
    {
        bool IsDone         { get; }    // 是否下载完成或已经取消
        bool IsSucceeded    { get; }    // 是否下载成功
    }
}