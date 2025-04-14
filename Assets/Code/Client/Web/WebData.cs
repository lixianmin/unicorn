/********************************************************************
created:    2022-12-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn.Web
{
    public class WebData : IWebNode
    {
        public WebState GetState() => _state;
        public Object Asset { get; set; }

        public void CopyFrom(IWebNode other)
        {
            if (other != null)
            {
                _state = other.GetState();
                Asset = other.Asset;
            }
        }

        public void Reset()
        {
            _state = WebState.None;
            Asset = null;
        }

        private WebState _state = WebState.None;
    }
}