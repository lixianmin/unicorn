/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal abstract class UIStateBase
    {
        public static UIStateBase Create(StateKind kind)
        {
            UIStateBase state = null;
            switch (kind)
            {
                case StateKind.None:
                    state = new NoneState();
                    break;
                case StateKind.Load:
                    state = new LoadState();
                    break;
                case StateKind.OpenAnimation:
                    state = new OpenAnimationState();
                    break;
                case StateKind.Opened:
                    state = new OpenedState();
                    break;
                case StateKind.Unload:
                    state = new UnloadState();
                    break;
                case StateKind.CloseAnimation:
                    state = new CloseAnimationState();
                    break;
                case StateKind.Closed:
                    state = new ClosedState();
                    break;
                case StateKind.Failure:
                    state = new FailureState();
                    break;
                default:
                    Logo.Error("invalid state kind={0}", kind);
                    break;
            }

            return state;
        }

        public virtual void OnEnter(WindowFetus fetus, object arg1) {}

        public virtual void OnExit(WindowFetus fetus, object arg1) {}

        public virtual void OnOpenWindow(WindowFetus fetus) {}
        public virtual void OnCloseWindow(WindowFetus fetus) {}

        protected static readonly UILoadingMask _loadWindowMask = new(0.5f);
        protected static readonly UILoadingMask _playAnimationMask = new(0);
    }
}