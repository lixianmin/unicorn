/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class UnloadState : StateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            AssertTools.IsTrue(!_isDelayedOpenWindow);
            var master = fetus.master;
            CallbackTools.Handle(master.InnerOnUnloading, "[OnEnterUnloadState()]");

            fetus.isLoaded = false;
            fetus.ChangeState(StateKind.None);
            master.Dispose();

            // 如果在关闭的过程中遇到了打开本window的请示，则在关闭后重新打开自己
            if (_isDelayedOpenWindow)
            {
                _isDelayedOpenWindow = false;
                UIManager.Instance.OpenWindow(master.GetType());
            }
            
            AssertTools.IsTrue(!_isDelayedOpenWindow);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedOpenWindow = true;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedOpenWindow = false;
        }
        
        private bool _isDelayedOpenWindow; // 遇到了OpenWindow()的请求
    }
}