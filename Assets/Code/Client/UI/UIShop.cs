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
            _loopScrollRect.UI.OnVisibleChanged += _OnCellVisibleChanged;
            for (int i = 0; i < 5; i++)
            {
                _loopScrollRect.UI.AddCell(i);
                Logo.Info(i.ToString());
            }
        }

        private void _OnCellVisibleChanged(UILoopScrollRect.Cell cell)
        {
            if (cell.IsVisible())
            {
                var data = (int)cell.GetData();
                var trans = cell.GetTransform();
                var title = trans.GetComponentInChildren<UIText>();
                title.text = "item: "+data.ToString();
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