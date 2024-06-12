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
        private static bool _IsWindowCacheable(WindowFetus fetus)
        {
            // return fetus._master.CACHE_HINT and LuaUITools.isBigMemoryMode
            return fetus.HasFlag(FetusFlags.Cache);
        }

        public override void OnEnter(WindowFetus fetus, object failureText)
        {
            if (_IsWindowCacheable(fetus))
            {
                fetus.SetActive(false);
            }
            else
            {
                fetus.ChangeState(StateKind.Unload);
            }
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