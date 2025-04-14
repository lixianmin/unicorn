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
        public WebStatus Status { get; set; }
        public Object Asset { get; set; }

        public void Cancel()
        {
        }

        public void CopyFrom(IWebNode other)
        {
            if (other != null)
            {
                Status = other.Status;
                Asset = other.Asset;
            }
        }

        public void Reset()
        {
            Status = WebStatus.None;
            Asset = null;
        }
    }
}