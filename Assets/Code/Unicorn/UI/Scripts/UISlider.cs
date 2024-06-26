
/********************************************************************
created:    2017-07-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine.UI;

namespace Unicorn.UI
{
    public class UISlider : Slider, IRemoveAllListeners
    {
        void IRemoveAllListeners.RemoveAllListeners()
        {
            onValueChanged.RemoveAllListeners();
        }
    }
}
