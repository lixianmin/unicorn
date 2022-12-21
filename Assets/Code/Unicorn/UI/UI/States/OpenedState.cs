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
            AssertTools.IsTrue(!_isDelayedClosing);
            var master = fetus.master;

            master.InnerOnOpened("[OnEnter()]");
            UIManager.Instance._SetForegroundWindow(master, master.GetRenderQueue());
            fetus.isOpened = true;

            // 如果在OnOpened()或OnActivated()中收到了CloseWindow()
            if (_isDelayedClosing)
            {
                _isDelayedClosing = false;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;
            // isOpened is used to judge whether the window can be activated, thus it must be set to be false as soon as OnExit() is called.
            fetus.isOpened = false;

            UIManager.Instance._OnClosingWindow(master);
            master.InnerOnClosing("[OnExit()]");
            AssertTools.IsTrue(!_isDelayedClosing);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedClosing = false;
            if (fetus.isOpened)
            {
                var master = fetus.master;
                UIManager.Instance._SetForegroundWindow(master, master.GetRenderQueue());
            }
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            if (fetus.isOpened)
            {
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else
            {
                _isDelayedClosing = true;
            }
        }

        private bool _isDelayedClosing;
    }
}