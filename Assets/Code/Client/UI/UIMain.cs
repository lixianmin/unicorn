/********************************************************************
created:    2022-08-17
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
    public class UIMain : UIWindowBase
    {
        public override string GetAssetPath()
        {
            return "Assets/res/prefabs/ui/uimain.prefab";
        }

        protected override void OnLoaded()
        {
            AtUnloading += _btnOpenBag.UI.onClick.On(() => { UIManager.It.OpenWindow(typeof(UIBag)); });

            AtUnloading += _btnCloseBag.UI.onClick.On(() => { UIManager.It.CloseWindow(typeof(UIBag)); });

            AtUnloading += _btnOpenShop.UI.onClick.On(() => { UIManager.It.OpenWindow(typeof(UIShop)); });

            AtUnloading += _btnCloseShop.UI.onClick.On(() => { UIManager.It.CloseWindow(typeof(UIShop)); });

            // Logo.Info(_btnCloseBagObject.UI.localPosition);
            Logo.Info("main is OnLoaded");
            // UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        protected override void OnOpened()
        {
            Logo.Info("main is OnOpened");
            // UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        protected override void OnActivated()
        {
            Logo.Info("main is OnActivated");
            // UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        protected override void OnDeactivating()
        {
            Logo.Info("main is OnDeactivating, state={0}, foreground={1}", GetFetus().GetState(),
                UIManager.It.GetForegroundWindow(GetRenderQueue()));
            // UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        protected override void OnClosing()
        {
            Logo.Info("main is OnClosing");
            // UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        protected override void OnUnloading()
        {
            Logo.Info("main is OnUnloading");
            UIManager.It.OpenWindow(GetType());
            // UIManager.It.CloseWindow(GetType());
        }

        private readonly UIWidget<UIButton> _btnOpenBag = new("btn_open_bag");
        private readonly UIWidget<UIButton> _btnCloseBag = new("btn_close_bag");
        private readonly UIWidget<UIButton> _btnOpenShop = new("btn_open_shop");
        private readonly UIWidget<UIButton> _btnCloseShop = new("btn_close_shop");
    }
}

#endif