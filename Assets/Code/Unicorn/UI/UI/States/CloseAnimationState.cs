/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class CloseAnimationState : StateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            AssertTools.IsTrue(!_isDelayedOpenWindow);
            
            var serializer = fetus.GetSerializer();
            if (serializer is not null)
            {
                _closeAnimation = serializer.closeWindowAnimation;
                _isPlaying = _closeAnimation != null;
            }
            
            if (_isPlaying)
            {
                _closeAnimation!.enabled = true;
                _=_closeAnimation.Init(() =>
                {
                    _OnCloseWindowAnimationDone(fetus);
                });
                
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

            if (_isDelayedOpenWindow)
            {
                _isDelayedOpenWindow = false;
                fetus.ChangeState(StateKind.OpenAnimation);
            }
            else
            {
                fetus.ChangeState(StateKind.Closed);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedOpenWindow = true;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedOpenWindow = false;
        }

        private UIWindowAnimation _closeAnimation;
        private bool _isPlaying;
        private bool _isDelayedOpenWindow; // 遇到了OpenWindow()的请求
    }
}