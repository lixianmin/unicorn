
/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#if UNICORN_EDITOR

using System;
using System.Collections;
using Unicorn;
using Unicorn.Web;
using YooAsset;

namespace Clients.Web
{
    using UObject = UnityEngine.Object;

    public class WebItem : Disposable, IWebNode
    {
        internal WebItem(WebArgument argument, Action<WebItem> handler)
        {
            argument.key ??= string.Empty;
            _argument = argument;
            CoroutineManager.It.StartCoroutine(_CoLoad(argument, handler), out _coroutineItem);
        }

        private IEnumerator _CoLoad(WebArgument argument, Action<WebItem> handler)
        {
            var loadHandle = YooAssets.LoadAssetAsync<UObject>(argument.key);
            _loadHandle = loadHandle;

            // LoadAssetAsync发生InvalidKeyException异常时，只会打印一条日志，不会真正的抛出异常
            while (!loadHandle.IsDone)
            {
                yield return null;
            }

            // 无论加载是否成功，都需要回调到handler
            IsDone = true;
            IsSucceeded = loadHandle.Status == EOperationStatus.Succeed;
            CallbackTools.Handle(ref handler, this, string.Empty);
        }

        protected override void _DoDispose(bool isManualDisposing)
        {
            _loadHandle.Dispose();
            _coroutineItem?.Kill();
            // Logo.Info($"[_DoDispose()] assetPath = {_argument.key}");
        }

        public bool IsDone { get; private set; }
        public bool IsSucceeded { get; private set; }
        public string Key => _argument.key;

        public UObject Asset
        {
            get
            {
                if (IsSucceeded)
                {
                    return _loadHandle.AssetObject;
                }

                return null;
            }
        }

        private readonly WebArgument _argument;
        private AssetHandle _loadHandle;
        private readonly CoroutineItem _coroutineItem;
    }
}

#endif