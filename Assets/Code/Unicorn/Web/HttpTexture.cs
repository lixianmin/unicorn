/********************************************************************
created:    2024-02-22
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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Unicorn.Web
{
    public class HttpTexture : Disposable, IWebNode
    {
        public HttpTexture(string url, Action<HttpTexture> fn = null)
        {
            if (url.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(url));
            }

            _url = url;
            Status = WebStatus.Loading;

            var request = UnityWebRequestTexture.GetTexture(url);
            CoroutineManager.It.StartCoroutine(_CoDownload(request, fn));

            AtDisposing += _AtDisposing;
        }

        private IEnumerator _CoDownload(UnityWebRequest request, Action<HttpTexture> fn)
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
            }

            Status = request.result == UnityWebRequest.Result.Success ? WebStatus.Succeeded : WebStatus.Failed;
            if (Status == WebStatus.Succeeded)
            {
                _texture = DownloadHandlerTexture.GetContent(request);
            }

            request.Dispose();
            CallbackTools.Handle(ref fn, this);
        }

        private void _AtDisposing()
        {
            if (_texture != null)
            {
                Object.Destroy(_texture);
                _texture = null;
            }

            Status = WebStatus.None;
        }

        public Texture2D GetTexture()
        {
            return _texture;
        }

        public string GetUrl()
        {
            return _url;
        }

        public void Cancel()
        {
        }

        public WebStatus Status { get; private set; }
        public Object Asset => _texture;

        private Texture2D _texture;
        private readonly string _url;
    }
}