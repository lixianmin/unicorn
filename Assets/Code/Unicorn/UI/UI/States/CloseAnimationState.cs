/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class CloseAnimationState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var serializer = fetus.GetSerializer();
            if (serializer is not null)
            {
                _closeAnimation = serializer.closeWindowAnimation;
                _isPlaying = _closeAnimation != null;
            }

            if (_isPlaying)
            {
                _closeAnimation!.enabled = true;
                _ = _closeAnimation.Init(() => { _OnCloseWindowAnimationDone(fetus); });

                _playAnimationMask.OpenWindow();
            }
            else
            {
                fetus.ChangeState(StateKind.Closed);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _closeAnimation = null;

            if (_isPlaying)
            {
                _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
        }

        private void _OnCloseWindowAnimationDone(WindowFetus fetus)
        {
            _playAnimationMask.CloseWindow();
            _isPlaying = false;
            _closeAnimation.SetEnabledEx(false);

            if (_delayedAction == DelayedAction.OpenWindow)
            {
                fetus.ChangeState(StateKind.OpenAnimation);
                _delayedAction = DelayedAction.None;
            }
            else // if (_delayedAction == DelayedAction.CloseWindow)
            {
                fetus.ChangeState(StateKind.Closed);
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

        private UIWindowAnimation _closeAnimation;
        private bool _isPlaying;
        private DelayedAction _delayedAction; // 遇到了OpenWindow()的请求
    }
}