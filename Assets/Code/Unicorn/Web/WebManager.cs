
/********************************************************************
created:    2022-08-12
author:     lixianmin

https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/RuntimeAddressables.html

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;

namespace Unicorn.Web
{
    public static class WebManager
    {
        public static WebItem LoadWebItem(string key, Action<WebItem> handler)
        {
            var argument = new WebArgument { key = key ?? string.Empty };
            var item = new WebItem(argument, handler);
            return item;
        }

        public static WebPrefab LoadWebPrefab(string key, Action<WebPrefab> handler)
        {
            var argument = new WebArgument { key = key ?? string.Empty };
            var prefab = new WebPrefab(argument, handler);
            return prefab;
        }
    }
}
