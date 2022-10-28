
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
        protected WebManager()
        {
            Instance = this;
        }
        
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
        /// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？
        /// 2022-10-22 原本Instance是由子类负责设置的，综合考虑后决定由基类自己做这件事，理由包括：
        ///     1. 把权利收回来，并且避免子类忘记
        ///     2. 因为子类需提供更多的方法，方便起见会添加：public new static readonly GameWebManager Instance = new(); 性能也会比基类中Property版的更好一些
        ///     3. 设计中，这就是一个singleton，不应该同时存在多个instance
        ///     4. 但是，Instance对象是lazy load的，如果基类的Instance先于子类的调用了，会报NullReferenceException
        /// </summary>
        public static WebManager Instance { get; private set; }
    }
}
