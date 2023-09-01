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
    public class UIShop : UIWindowBase
    {
        public override string GetAssetPath()
        {
            return "Assets/res/prefabs/ui/uishop.prefab";
        }

        protected override void OnLoaded()
        {
            _ReloadWidgets();

            _dog.AddListener(ShopManager.It.OnInsertGoods, _OnInsertGoods);
            _dog.AddListener(ShopManager.It.OnDeleteGoods, _OnDeleteGoods);
        }

        private void _ReloadWidgets()
        {
            _loopScrollRect.UI.RemoveAllCells();
            foreach (var goods in ShopManager.It.GetEnumerator())
            {
                var widget = new UIShopWidget(goods);
                _loopScrollRect.UI.AddCell(widget);
            }
        }

        private void _OnInsertGoods(ShopGoods goods)
        {
            var widget = new UIShopWidget(goods);
            _loopScrollRect.UI.AddCell(widget);
        }

        private void _OnDeleteGoods(ShopGoods goods)
        {
            var ui = _loopScrollRect.UI;
            var count = ui.GetCellCount();
            for (int i = 0; i < count; i++)
            {
                var cell = ui.GetCell(i);
                var widget = cell.GetWidget() as UIShopWidget;
                if (widget!.GetTemplateId() == goods.GetTemplateId())
                {
                    ui.RemoveCell(i);
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