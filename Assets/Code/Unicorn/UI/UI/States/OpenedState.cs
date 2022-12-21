/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class OpenedState : StateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            AssertTools.IsTrue(!_isDelayedOpenWindow);
            var master = fetus.master;

            CallbackTools.Handle(master.InnerOnOpened, "[OnEnter()]");
            UIManager.Instance._SetForegroundWindow(master, master.GetRenderQueue());
            fetus.isOpened = true;
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;
            // isOpened is used to judge whether the window can be activated, thus it must be set to be false as soon as OnExit() is called.
            fetus.isOpened = false;
           
            UIManager.Instance._OnClosingWindow(master);
            CallbackTools.Handle(master.InnerOnClosing, "[OnExit()]");
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.isOpened)
            {
                var master = fetus.master;
                UIManager.Instance._SetForegroundWindow(master, master.GetRenderQueue());
            }
            else
            { // If the same window is opened again in OnClosing() or _onBeforeOpened
                _isDelayedOpenWindow = true;
            }
        }

        public override  void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedOpenWindow = false;
            if (fetus.isOpened)
            {
                fetus.ChangeState(StateKind.CloseAnimation);
            }
        }
        
        private bool _isDelayedOpenWindow; // 遇到了OpenWindow()的请求
    }
}