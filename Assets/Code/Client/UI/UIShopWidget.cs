/********************************************************************
created:    2022-08-32
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using Unicorn;
using Unicorn.UI;
using UnityEngine;

namespace Clients.UI
{
    public class UIShopWidget : UILoopScrollRect.IWidget
    {
        public UIShopWidget(ShopGood good)
        {
            _good = good;
        }

        public void OnVisibleChanged(UILoopScrollRect.Cell cell)
        {
            if (cell.IsVisible())
            {
                _rect = cell.GetTransform();
                var title = _rect.GetComponentInChildren<UIText>();
                title.text = "item: " + _good.GetTemplateId();

                var btn = _rect.GetComponentInChildren<UIButton>();
                _dog.AddListener(btn.onClick, _OnClickButton);
                _dog.AddListener(_good.OnUpdateGoods, _OnUpdateGoods);
            }
            else
            {
                _dog.RemoveAllListeners();
            }
        }

        private void _OnClickButton()
        {
            var nextId = ShopManager.It.GetNextId();
            _good.SetName(nextId.ToString());
            
            // ShopManager.It.DeleteGoods(_tid);
            // ShopManager.It.InsertGoods(nextId);
        }

        private void _OnUpdateGoods(ShopGood good)
        {
            var image = _rect.GetComponentInChildren<UIImage>();
            // 如果一个cell隐藏又显示了, 因为有transform的交换, 可能会导致变色
            image.color = image.color == Color.white ? Color.red : Color.white;
        }

        public int GetTemplateId()
        {
            return _good.GetTemplateId();
        }
        
        private readonly ShopGood _good;
        private RectTransform _rect;

        private readonly EventDog _dog = new();
    }
}

#endif