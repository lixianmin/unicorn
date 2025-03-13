/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class ClosedState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object failureText)
        {
            fetus.ChangeState(StateKind.Unload);
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn("[ClosedState.OnOpenWindow()]");    
            }
            
            if (fetus.HasFlag(FetusFlags.Cache))
            {
                fetus.SetActive(true);
            }

            fetus.ChangeState(StateKind.OpenAnimation);
        }
    }
}