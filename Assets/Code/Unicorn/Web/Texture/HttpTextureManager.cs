/********************************************************************
created:    2024-02-28
author:     lixianmin

目前使用 Hashtable 作为缓存容器, 如果有一天这个缓存需要改为LRU之类的策略缓存, 可以
考虑使用System.Runtime.Caching.MemoryCache, 但这个类似乎只能基于NuGet去下载

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

namespace Unicorn.Web
{
    public class HttpTextureManager
    {
        private HttpTextureManager()
        {
        }

        public HttpTexture LoadTexture(string url, Action<HttpTexture> fn = null)
        {
            _textureLoader ??= new BuiltinTextureLoader();
            return _textureLoader.LoadTexture(url, fn);
        }

        public void SetTextureLoader(ITextureLoader textureLoader)
        {
            _textureLoader = textureLoader;
        }

        private ITextureLoader _textureLoader;

        public static readonly HttpTextureManager It = new();
    }
}