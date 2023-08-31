/********************************************************************
created:    2022-08-32
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using Unicorn.UI;

namespace Client.UI
{
    public class UIShopCell : UILoopScrollRect.ICellData
    {
        public UIShopCell(int index)
        {
            _index = index;
        }

        public void OnVisibleChanged(UILoopScrollRect.Cell cell)
        {
            if (cell.IsVisible())
            {
                var data = (UIShopCell)cell.GetData();
                var trans = cell.GetTransform();
                var title = trans.GetComponentInChildren<UIText>();
                title.text = "item: " + data._index.ToString();
            }
        }

        private int _index;
    }
}

#endif