/********************************************************************
created:    2022-08-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#if UNICORN_EDITOR

using Unicorn;
using Unicorn.UI;

namespace Client.UI
{
    public class UIShop: UIWindowBase
    {
        public override string GetAssetPath()
        {
            return "uishop";
        }

        protected override void OnLoaded()
        {
            _dog.AddListener(_btnClose.UI.onClick, _OnClickButtonClose);
        }
        

        protected override void OnUnloading()
        {
            _dog.RemoveAllListeners();
        }

        private void _OnClickButtonClose()
        {
            
        }

        private readonly EventDog _dog = new();
        private readonly UIWidget<UIButton> _btnClose = new("btn_close");
    }
}

#endif