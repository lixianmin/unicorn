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
        public bool IsDone { get; set; }
        public bool IsSucceeded { get; set; }
        public Object Asset { get; set; }

        public void CopyFrom(IWebNode other)
        {
            if (other != null)
            {
                IsDone = other.IsDone;
                IsSucceeded = other.IsSucceeded;
                Asset = other.Asset;
            }
        }

        public void Reset()
        {
            IsDone = false;
            IsSucceeded = false;
            Asset = null;
        }
    }
}