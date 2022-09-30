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
    internal class MBPrefabAid : MonoBehaviour
    {
        // Caution: Awake() will not be automatically called immediately 
        // when _mainAsset is not active.
        private void Awake ()
        {
            Init();
        }

        internal void Init ()
        {
            if (!_isInited)
            {
                _isInited = true;
                PrefabRecycler.AddReference(key);
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