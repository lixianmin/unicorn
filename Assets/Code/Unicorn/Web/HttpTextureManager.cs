/********************************************************************
created:    2024-02-28
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
using UnityEngine.SceneManagement;

namespace Unicorn.Web
{
    internal class HttpTextureManager
    {
        private HttpTextureManager()
        {
            SceneManager.sceneLoaded += _OnSceneLoaded;
        }

        private void _OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var num = _textures.Count;
            _textures.Clear();
            Logo.Info($"[_OnSceneLoaded] removed _textures.Count={num}");
        }

        public HttpTexture LoadTexture(string url, Action<HttpTexture> fn = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                CallbackTools.Handle(ref fn, null);
                return null;
            }

            // 如果缓存的HttpTexture还没有加载完成, 则不能调用fn()
            if (_textures[url] is HttpTexture cachedTexture && cachedTexture.GetTexture() != null)
            {
                CallbackTools.Handle(ref fn, cachedTexture);
                return cachedTexture;
            }

            var texture = new HttpTexture(url, fn);
            _textures[url] = texture;
            texture.AtDisposing += () => { _textures.Remove(url); };

            return texture;
        }

        private readonly Hashtable _textures = new();

        public static readonly HttpTextureManager It = new();
    }
}