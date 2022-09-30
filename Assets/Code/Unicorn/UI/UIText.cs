/********************************************************************
created:    2022-08-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using TMPro;
using UnityEngine;

namespace Unicorn.UI
{
    public class UIText : TextMeshProUGUI
    {
        public string GetGUID ()
        {
            return _guid;
        }

        [SerializeField] private string _guid = string.Empty;
    }
}