/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using TMPro;

namespace Unicorn.UI
{
    public class UIInputField : TMP_InputField
    {
        protected override void OnDestroy()
        {
            onValueChanged.RemoveAllListeners();
            onEndEdit.RemoveAllListeners();
            onSelect.RemoveAllListeners();
            onDeselect.RemoveAllListeners();
            
            base.OnDestroy();
        }
    }
}