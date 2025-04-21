/********************************************************************
created:    2022-08-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using Unicorn;
using Unicorn.UI;
using UnityEngine;

namespace Clients.UI
{
    public class UIShop : UIWindowBase
    {
        public override string GetAssetPath()
        {
            return "Assets/res/prefabs/ui/uishop.prefab";
        }

        protected override void OnLoaded()
        {
            _ReloadWidgets();

            _dog.AddListener(ShopManager.It.OnInsertGood, _OnInsertGoods);
            _dog.AddListener(ShopManager.It.OnDeleteGood, _OnDeleteGoods);
            Logo.Warn($"[OnLoaded] frameCount={Time.frameCount}");
        }

        protected override void OnOpened()
        {
            Logo.Warn($"[OnOpened] frameCount={Time.frameCount}");
        }

        protected override void OnActivated()
        {
            Logo.Warn($"[OnActivated] frameCount={Time.frameCount}");
        }

        private void _ReloadWidgets()
        {
            _loopScrollRect.UI.RemoveAll();

            foreach (var good in ShopManager.It.GetGoods())
            {
                var widget = new UIShopWidget(good);
                _loopScrollRect.UI.AddWidget(widget);
                Logo.Info($"tid={good.GetTemplateId()}, good={good}");
            }
        }

        private void _OnInsertGoods(ShopGood good)
        {
            var widget = new UIShopWidget(good);
            _loopScrollRect.UI.AddWidget(widget);
        }

        private void _OnDeleteGoods(ShopGood good)
        {
            var ui = _loopScrollRect.UI;
            var count = ui.GetCount();
            for (int i = 0; i < count; i++)
            {
                var cell = ui.GetCell(i);
                var widget = cell.GetWidget() as UIShopWidget;
                if (widget!.GetTemplateId() == good.GetTemplateId())
                {
                    ui.RemoveWidget(i);
                    break;
                }
            }
        }

        protected override void OnUnloading()
        {
            _dog.RemoveAllListeners();
            Logo.Info("uishop is unloading");
        }

        private readonly EventDog _dog = new();
        private readonly UIWidget<UILoopScrollRect> _loopScrollRect = new("shop_view");
    }
}

#endif