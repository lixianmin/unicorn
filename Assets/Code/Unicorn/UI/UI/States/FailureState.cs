/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class FailureState : StateBase
    {
        public override void OnEnter(WindowFetus fetus, object failureText)
        {
            Logo.Error("Enter FailureState, failureText={0}", failureText);
            fetus.ChangeState(StateKind.None);
            fetus.master.Dispose();
        }
    }
}