/********************************************************************
created:    2022-11-24
author:     lixianmin


Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.Web
{
    public class EmptyWebNode : IWebNode, IDisposable
    {
        private EmptyWebNode(WebStatus status)
        {
            Status = status;
        }

        public void Cancel()
        {
        }

        void IDisposable.Dispose()
        {
        }

        public WebStatus Status { get; }

        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object IWebNode.Asset => null;

        public static readonly EmptyWebNode Succeeded = new(WebStatus.Succeeded);
        public static readonly EmptyWebNode Failed = new(WebStatus.Failed);
    }
}