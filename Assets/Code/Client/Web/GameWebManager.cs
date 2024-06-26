/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
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