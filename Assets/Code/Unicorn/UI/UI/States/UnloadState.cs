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
            AssertTools.IsTrue(!_isDelayedOpening);
            var master = fetus.master;
            master.InnerOnUnloading("[OnEnterUnloadState()]");

            fetus.isLoaded = false;
            fetus.ChangeState(StateKind.None);
            master.Dispose();

            // 如果在关闭的过程中遇到了打开本window的请示，则在关闭后重新打开自己
            if (_isDelayedOpening)
            {
                _isDelayedOpening = false;
                UIManager.Instance.OpenWindow(master.GetType());
            }
            
            AssertTools.IsTrue(!_isDelayedOpening);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedOpening = true;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedOpening = false;
        }
        
        private bool _isDelayedOpening; // 遇到了OpenWindow()的请求
    }
}