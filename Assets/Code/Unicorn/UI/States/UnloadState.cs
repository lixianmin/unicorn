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
            AssertTools.IsTrue(!fetus.isDelayedOpenWindow);
            var master = fetus.master;
            CallbackTools.Handle(master.OnUnloading, "[OnEnter()]");

            fetus.isLoaded = false;
            fetus.ChangeState(StateKind.None);
            master.Dispose();

            if (fetus.isDelayedOpenWindow)
            {
                fetus.isDelayedOpenWindow = false;
                // UIManager.OpenWindow();
            }
            
            AssertTools.IsTrue(!fetus.isDelayedOpenWindow);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            fetus.isDelayedOpenWindow = true;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            fetus.isDelayedOpenWindow = false;
        }
    }
}