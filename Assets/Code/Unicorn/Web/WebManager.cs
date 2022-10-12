
/********************************************************************
created:    2022-08-12
author:     lixianmin

https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/RuntimeAddressables.html

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn.Web
{
    public class WebManager
    {
        public WebItem LoadWebItem(string key, Action<WebItem> handler)
        {
            var argument = new WebArgument { key = key ?? string.Empty };
            var item = new WebItem(argument, handler);
            return item;
        }

        public WebPrefab LoadWebPrefab(string key, Action<WebPrefab> handler)
        {
            var argument = new WebArgument { key = key ?? string.Empty };
            var prefab = new WebPrefab(argument, handler);
            return prefab;
        }
        
        /// <summary>
        /// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？
        /// </summary>
        public static WebManager Instance { get; protected set; }
    }
}
