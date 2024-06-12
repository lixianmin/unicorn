/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class NoneState : UIStateBase
    {
        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn("[NoneState.OnOpenWindow()]");    
            }
            
            fetus.ChangeState(StateKind.Load);
        }
    }
}