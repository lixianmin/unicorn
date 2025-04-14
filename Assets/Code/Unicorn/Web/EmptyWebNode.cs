/********************************************************************
created:    2022-11-24
author:     lixianmin


Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Web
{
    public class EmptyWebNode : IWebNode
    {
        public void Cancel()
        {
            
        }

        public WebStatus Status => WebStatus.Succeeded;

        /// <summary>
        /// 返回加载的资源对象
        /// </summary>
        UnityEngine.Object IWebNode.Asset => null;

        public static readonly EmptyWebNode It = new();
    }
}