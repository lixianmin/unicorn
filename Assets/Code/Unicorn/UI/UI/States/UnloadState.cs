/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class UnloadState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;
            master.InnerOnUnloading("[OnEnterUnloadState()]");

            var isDelayedOpening = _delayedAction == DelayedAction.OpenWindow;
            fetus.RemoveFlag(FetusFlags.Loaded);
            fetus.ChangeState(StateKind.None);
            master.Dispose(); // 这个会设置_isDelayedOpening=false;

            // 如果在关闭的过程中遇到了打开本window的请示，则在关闭后重新打开自己
            if (isDelayedOpening)
            {
                UIManager.It.OpenWindow(master.GetType());
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _delayedAction = DelayedAction.OpenWindow;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _delayedAction = DelayedAction.CloseWindow;
        }

        private DelayedAction _delayedAction; // 遇到了OpenWindow()的请求
    }
}