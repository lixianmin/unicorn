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
using Unicorn.Web;

namespace Clients.Web
{
    public partial class GameWebManager : WebManager
    {
        static GameWebManager()
        {
        }

        private GameWebManager()
        {
        }

        public IWebNode LoadAsset(string key, Action<IWebNode> handler)
        {
            var argument = new WebArgument { key = key };
            var webItem = new WebItem(argument, handler);
            return webItem;
        }
        
        public override IWebNode LoadAsset(WebArgument argument, Action<IWebNode> handler)
        {
            var webItem = new WebItem(argument, handler);
            return webItem;
        }
        
        public WebPrefab LoadPrefab(string key, Action<WebPrefab> handler)
        {
            var argument = new WebArgument { key = key };
            var prefab = LoadPrefab(argument, handler);
            return prefab;
        }
        
        public WebScene LoadScene(string key, Action<WebScene> handler)
        {
            var argument = new WebArgument { key = key };
            var webItem = new WebScene(argument, handler);
            return webItem;
        }
        
        public WebScene LoadScene(WebArgument argument, Action<WebScene> handler)
        {
            var webItem = new WebScene(argument, handler);
            return webItem;
        }

        public new static readonly GameWebManager It = new();
    }
}

#endif