/********************************************************************
created:    2015-01-07
author:     lixianmin

1. 不知道什麼原因，bundle上掛接了script後（無論該script是否引用了該bundle)，
	都會導致包含該bundle.mainAsset的InnerWebPrefab對象無法被gc回收）

2. 2015-05-29 optimize: 在OnDestroy()时，使用WebCacheTools.RemoveFromCache() 
	代替原本单纯的inner.RemoveReference()，在条件允许的情况下可以立即释放
	占用的内存（比如大场景对象）

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;

namespace Unicorn.Web.Internal
{
    public class MbPrefabAid : MonoBehaviour
    {
        // 1. Caution: Awake() will not be automatically called immediately when _mainAsset is not active.
        // 2. Awake()与OnDestroy()都是最多执行一次
        // 3. ~~Init()可以最多执行2次~~
        // 4. 无论是否执行过Init(), 调用OnDestroy()都不会有问题
        private void Awake ()
        {
            Init();
        }

        private void Init ()
        {
            // 这个方法没必要在WebPrefab加载完成的时候调用一次, 因为WebPrefab会在OnLoaded()/Dispose()中处理自己的引用计数
            if (!_isInited)
            {
                _isInited = true;
                PrefabRecycler.AddReference(key);
                // Logo.Info($"instanceId={GetInstanceID()}");
            }
        }

        private void OnDestroy ()
        {
            if (_isInited)
            {
               PrefabRecycler.RemoveReference(key);
            }
        }

        // this variable is declared as public because we need it to be copied when gameObject is cloned.
        public string key;

        private bool _isInited;
    }
}