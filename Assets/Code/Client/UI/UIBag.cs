/********************************************************************
created:    2022-08-16
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

namespace Clients.UI
{
    public class UIBag : UIWindowBase
    {
        public override string GetAssetPath()
        {
            return "uibag";
        }

        public override RenderQueue GetRenderQueue()
        {
            return RenderQueue.Geometry;
        }

        protected override void OnLoaded()
        {
            AtUnloading += _btnClickBag.UI.onClick.On(() =>
            {
                Logo.Info("bag button is clicked");
                UIManager.It.CloseWindow(typeof(UIMain));
            });

            Logo.Info("bag is OnLoaded");
        }

        protected override void OnOpened()
        {
            Logo.Info("bag is OnOpened");
        }

        protected override void OnActivated()
        {
            Logo.Info("bag is OnActivated");
        }

        protected override void OnDeactivating()
        {
            Logo.Info("bag is OnDeactivating");
        }

        protected override void OnClosing()
        {
            Logo.Info("bag is OnClosing");
        }

        protected override void OnUnloading()
        {
            Logo.Info("bag is OnUnloading");
        }

        private readonly UIWidget<UIButton> _btnClickBag = new("btn_click_bag");
    }
}

#endif