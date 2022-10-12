
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
        /// <summary>
        /// 基础加载逻辑，加载单个AssetBundle或其它asset文件。实现逻辑需要确保只要IWebNode对象存在，则对应的assetBundle不会被gc回收
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public abstract IWebNode LoadAsset(WebArgument argument, Action<IWebNode> handler);

        /// <summary>
        /// 加载一个Prefab对象。只要WebPrefab对象或clone出来的gameObject对象存在，则对应的AssetBundle不会被gc回收
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public WebPrefab LoadPrefab(WebArgument argument, Action<WebPrefab> handler)
        {
            var prefab = new WebPrefab(argument, handler);
            return prefab;
        }
        
        /// <summary>
        /// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？子类需要负责设置Instance的值
        /// </summary>
        public static WebManager Instance { get; protected set; }
    }
}
