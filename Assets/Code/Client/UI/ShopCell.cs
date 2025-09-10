/********************************************************************
created:    2022-08-32
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

#if UNICORN_EDITOR

using System;
using Unicorn;
using Unicorn.UI;
using UnityEngine;

namespace Clients.UI
{
    public class ShopCell : UILoopScrollRect.CellBase
    {
        public ShopCell(ShopGood good)
        {
            _good = good;
        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible())
            {
                _rect = GetTransform();
                var title = _rect.GetComponentInChildren<UIText>();
                title.text = "item: " + _good.GetTemplateId();

                var btn = _rect.GetComponentInChildren<UIButton>();
                AtChanged += btn.onClick.On(_OnClickButton);
                AtChanged += _good.OnUpdateGoods.On(_OnUpdateGoods);
            }
            else
            {
                AtChanged?.Invoke();
                AtChanged = null;
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
            // var image = _rect.GetComponentInChildren<UIImage>();
            // // 如果一个cell隐藏又显示了, 因为有transform的交换, 可能会导致变色
            // image.color = image.color == Color.white ? Color.red : Color.white;
        }

        public int GetTemplateId()
        {
            return _good.GetTemplateId();
        }

        private readonly ShopGood _good;
        private RectTransform _rect;

        public event Action AtChanged;
    }
}

#endif