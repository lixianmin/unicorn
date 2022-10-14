
/********************************************************************
created:    2017-07-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine.UI;

namespace Unicorn.UI
{
    public class UIToggle : Toggle
    {
        protected override void OnDestroy()
        {
            onValueChanged.RemoveAllListeners();
            base.OnDestroy();
        }
    }
}