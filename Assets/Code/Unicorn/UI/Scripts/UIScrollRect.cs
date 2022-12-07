/********************************************************************
created:    2022-12-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine.UI;

namespace Unicorn.UI
{
    public class UIScrollRect : ScrollRect, IRemoveAllListeners
    {
        void IRemoveAllListeners.RemoveAllListeners()
        {
            onValueChanged.RemoveAllListeners();
        }
    }
}