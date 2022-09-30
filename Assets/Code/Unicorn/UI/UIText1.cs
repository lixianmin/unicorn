/********************************************************************
created:    2017-07-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.UI
{
    public class UIText1 : Text
    {
        internal void AssignDefaultFont ()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        public string GetGUID ()
        {
            return _guid;
        }

        [SerializeField] private string _guid = string.Empty;
    }
}