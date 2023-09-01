/********************************************************************
created:    2022-08-32
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using Unicorn;
using Unicorn.UI;
using UnityEngine;

namespace Client.UI
{
    public class UIShopWidget : UILoopScrollRect.IWidget
    {
        public UIShopWidget(ShopGoods goods)
        {
            _goods = goods;
        }

        public void OnVisibleChanged(UILoopScrollRect.Cell cell)
        {
            if (cell.IsVisible())
            {
                _rect = cell.GetTransform();
                var title = _rect.GetComponentInChildren<UIText>();
                title.text = "item: " + _goods.GetTemplateId();

                var btn = _rect.GetComponentInChildren<UIButton>();
                _dog.AddListener(btn.onClick, _OnClickButton);
                _dog.AddListener(_goods.OnUpdateGoods, _OnUpdateGoods);
            }
            else
            {
                _dog.RemoveAllListeners();
            }
        }

        private void _OnClickButton()
        {
            var nextId = ShopManager.It.GetNextId();
            _goods.SetName(nextId.ToString());
            
            // ShopManager.It.DeleteGoods(_tid);
            // ShopManager.It.InsertGoods(nextId);
        }

        private void _OnUpdateGoods(ShopGoods goods)
        {
            var image = _rect.GetComponentInChildren<UIImage>();
            // 如果一个cell隐藏又显示了, 因为有transform的交换, 可能会导致变色
            image.color = image.color == Color.white ? Color.red : Color.white;
        }

        public int GetTemplateId()
        {
            return _goods.GetTemplateId();
        }
        
        private readonly ShopGoods _goods;
        private RectTransform _rect;

        private readonly EventDog _dog = new();
    }
}

#endif