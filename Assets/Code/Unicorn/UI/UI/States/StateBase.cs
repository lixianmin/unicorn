/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn.Collections;
using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal abstract class StateBase
    {
        public static StateBase Create(StateKind kind)
        {
            if (_states.TryGetValue(kind, out var last))
            {
                return last;
            }

            switch (kind)
            {
                case StateKind.None:
                    last = new NoneState();
                    break;
                case StateKind.Load:
                    last = new LoadState();
                    break;
                case StateKind.OpenAnimation:
                    last = new OpenAnimationState();
                    break;
                case StateKind.Opened:
                    last = new OpenedState();
                    break;
                case StateKind.Unload:
                    last = new UnloadState();
                    break;
                case StateKind.CloseAnimation:
                    last = new CloseAnimationState();
                    break;
                case StateKind.Closed:
                    last = new ClosedState();
                    break;
                case StateKind.Failure:
                    last = new FailureState();
                    break;
                default:
                    Console.Error.WriteLine("invalid state kind={0}", kind);
                    break;
            }

            _states[kind] = last;
            return last;
        }

        public virtual void OnEnter(WindowFetus fetus, object arg1) {}

        public virtual void OnExit(WindowFetus fetus, object arg1) {}

        public virtual void OnOpenWindow(WindowFetus fetus) {}
        public virtual void OnCloseWindow(WindowFetus fetus) {}

        private static readonly SortedTable<StateKind, StateBase> _states = new(8);
        protected static readonly UILoadingMask _loadWindowMask = new(0.5f);
        protected static readonly UILoadingMask _playAnimationMask = new(0);
    }
}