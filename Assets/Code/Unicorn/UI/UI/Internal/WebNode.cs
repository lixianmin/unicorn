/********************************************************************
created:    2022-12-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Web;
using UnityEngine;

namespace Unicorn.UI.Internal
{
    internal class WebNode : IWebNode
    {
        public bool IsCompleted { get; private set; }
        public bool IsSucceeded { get; private set; }
        public Object Asset { get; private set; }

        public void CopyProperty(IWebNode other)
        {
            if (other != null)
            {
                IsCompleted = other.IsCompleted;
                IsSucceeded = other.IsSucceeded;
                Asset = other.Asset;    
            }
        }
        
        public void Reset()
        {
            IsCompleted = false;
            IsSucceeded = false;
            Asset = null;
        }
    }
}