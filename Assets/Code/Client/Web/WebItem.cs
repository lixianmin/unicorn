/********************************************************************
created:    2022-08-12
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
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
            _status = WebStatus.Loading;
            CoroutineManager.It.StartCoroutine(_CoLoad(argument, handler), out _coroutineItem);

            AtDisposing += _AtDisposing;
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
            _status = loadHandle.Status == EOperationStatus.Succeed ? WebStatus.Succeeded : WebStatus.Failed;
            CallbackTools.Handle(ref handler, this, string.Empty);
        }

        private void _AtDisposing()
        {
            _loadHandle.Dispose();
            _coroutineItem?.Kill();
            // Logo.Info($"[_DoDispose()] assetPath = {_argument.key}");
        }

        public void Cancel()
        {
        }

        public string Key => _argument.key;

        public WebStatus Status => _status;

        public UObject Asset
        {
            get
            {
                if (_status == WebStatus.Succeeded)
                {
                    return _loadHandle.AssetObject;
                }

                return null;
            }
        }

        private readonly WebArgument _argument;
        private AssetHandle _loadHandle;
        private readonly CoroutineItem _coroutineItem;
        private WebStatus _status;
    }
}

#endif