/********************************************************************
created:    2022-08-28
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

            AtUnloading += ShopManager.It.OnInsertGood.On(_OnInsertGoods);
            AtUnloading += ShopManager.It.OnDeleteGood.On(_OnDeleteGoods);
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
            Logo.Info("uishop is unloading");
        }

        private readonly UIWidget<UILoopScrollRect> _loopScrollRect = new("shop_view");
    }
}

#endif