/********************************************************************
created:    2022-08-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using TMPro;

namespace Unicorn.UI
{
    public class UIInputField : TMP_InputField, IRemoveAllListeners
    {
        void IRemoveAllListeners.RemoveAllListeners()
        {
            onValueChanged.RemoveAllListeners();
            onEndEdit.RemoveAllListeners();
            onSelect.RemoveAllListeners();
            onDeselect.RemoveAllListeners();
        }
    }
}