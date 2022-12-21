/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class OpenAnimationState: StateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var serializer = fetus.GetSerializer();
            if (serializer is not null)
            {
                _openAnimation = serializer.openWindowAnimation;
                _isPlaying = _openAnimation != null;
            }
            
            if (_isPlaying)
            {
                _openAnimation!.enabled = true;
                _=_openAnimation.Init(() =>
                {
                    _OnOpenWindowAnimationDone(fetus);
                });
                _playAnimationMask.OpenWindow();
            }
            else
            {
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _openAnimation = null;

            if (_isPlaying)
            {
                _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
            
            AssertTools.IsTrue(!_isDelayedClosing);
        }

        private void _OnOpenWindowAnimationDone(WindowFetus fetus)
        {
            _playAnimationMask.CloseWindow();
            _isPlaying = false;
            _openAnimation.SetEnabledEx(false);
            
            if (_isDelayedClosing)
            {
                _isDelayedClosing = false;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else
            {
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            _isDelayedClosing = false;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _isDelayedClosing = true;
        }

        private UIWindowAnimation _openAnimation;
        private bool _isPlaying;
        private bool _isDelayedClosing; // 遇到了CloseWindow()的请示
    }
}