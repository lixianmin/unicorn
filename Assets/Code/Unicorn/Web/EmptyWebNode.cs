/********************************************************************
created:    2022-11-24
author:     lixianmin


Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    internal class EmptyWebNode:IWebNode
    {
        /// <summary>
        /// 下载过程是否已结束：成功/失败/取消
        /// </summary>
        public bool IsDone => true;

        /// <summary>
        /// 是否下载成功
        /// </summary>
        public bool IsSucceeded => false;

        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object IWebNode.Asset => null;

        public static readonly EmptyWebNode Instance = new();
    }
}