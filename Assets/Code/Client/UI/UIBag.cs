/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#if UNICORN_EDITOR

using Unicorn;
using Unicorn.UI;

namespace Client.UI
{
    public class UIBag: UIWindowBase
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
            _btnClickBag.UI.onClick.AddListener(() =>
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