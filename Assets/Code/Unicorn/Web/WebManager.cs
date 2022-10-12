
/********************************************************************
created:    2022-08-12
author:     lixianmin

https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/RuntimeAddressables.html

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn.Web
{
    public abstract class WebManager
    {
        public abstract IWebNode LoadAsset(WebArgument argument, Action<IWebNode> handler);

        public WebPrefab LoadPrefab(WebArgument argument, Action<WebPrefab> handler)
        {
            var prefab = new WebPrefab(argument, handler);
            return prefab;
        }
        
        /// <summary>
        /// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？
        /// </summary>
        public static WebManager Instance { get; protected set; }
    }
}
