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
        public UIShopWidget(int tid)
        {
            _tid = tid;
        }

        public void OnVisibleChanged(UILoopScrollRect.Cell cell)
        {
            if (cell.IsVisible())
            {
                _rect = cell.GetTransform();
                var title = _rect.GetComponentInChildren<UIText>();
                title.text = "item: " + _tid;

                var btn = _rect.GetComponentInChildren<UIButton>();
                _dog.AddListener(btn.onClick, _OnClickButton);
                _dog.AddListener(ShopManager.It.OnUpdateGoods, _OnUpdateGoods);
            }
            else
            {
                _dog.RemoveAllListeners();
            }
        }

        private void _OnClickButton()
        {
            var nextId = ShopManager.It.GetNextId();
            ShopManager.It.UpdateGoods(_tid, nextId.ToString());
            
            // ShopManager.It.DeleteGoods(_tid);
            
            // ShopManager.It.InsertGoods(nextId);
        }

        private void _OnUpdateGoods(ShopGoods goods)
        {
            if (goods.GetTemplateId() == _tid)
            {
                var image = _rect.GetComponentInChildren<UIImage>();
                image.color = image.color == Color.white ? Color.red : Color.white;
            }
        }

        public int GetTemplateId()
        {
            return _tid;
        }

        private int _tid;
        private RectTransform _rect;

        private readonly EventDog _dog = new();
    }
}

#endif