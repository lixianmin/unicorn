/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;

namespace Unicorn.UI.States
{
    internal class OpenAnimationState : UIStateBase
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
                _ = _openAnimation.Init(() => { _OnOpenWindowAnimationDone(fetus); });
                // _playAnimationMask.OpenWindow();
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
                // _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
        }

        private void _OnOpenWindowAnimationDone(WindowFetus fetus)
        {
            // _playAnimationMask.CloseWindow();
            _isPlaying = false;
            _openAnimation.SetEnabledEx(false);

            if (_delayedAction == DelayedAction.CloseWindow)
            {
                _delayedAction = DelayedAction.None;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else // if (_delayedAction == DelayedAction.OpenWindow)
            {
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn($"[OpenAnimationState.OnOpenWindow()] assetPath={fetus.GetAssetPath()}");
            }

            _delayedAction = DelayedAction.OpenWindow;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            if (!_isPlaying)
            {
                _delayedAction = DelayedAction.None;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else
            {
                _delayedAction = DelayedAction.CloseWindow;
            }

            if (fetus.IsDebugging())
            {
                Logo.Warn($"[OpenAnimationState.OnCloseWindow()] assetPath={fetus.GetAssetPath()}");
            }
        }

        private UIWindowAnimation _openAnimation;
        private bool _isPlaying;
        private DelayedAction _delayedAction; // 遇到了CloseWindow()的请示
    }
}