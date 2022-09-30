/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class NoneState : StateBase
    {
        public override void OnOpenWindow(WindowFetus fetus)
        {
            fetus.ChangeState(StateKind.Load);
        }
    }
}