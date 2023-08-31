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
            return "Assets/res/prefabs/ui/uishop.prefab";
        }

        protected override void OnLoaded()
        {
            for (int i = 0; i < 5; i++)
            {
                _loopScrollRect.UI.AddCell(new UIShopCell(i));
                Logo.Info(i.ToString());
            }
        }
        
        protected override void OnUnloading()
        {
            _dog.RemoveAllListeners();
            _loopScrollRect.UI.RemoveAllCells();
        }

        private readonly EventDog _dog = new();
        private readonly UIWidget<UILoopScrollRect> _loopScrollRect = new("shop_view");
    }
}

#endif