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
            AssertTools.IsTrue(!fetus.isDelayedCloseWindow);
            var serializer = fetus.GetSerializer();
            if (serializer is not null)
            {
                var script = serializer.openWindowScript;
                var evt = serializer.onOpenWindowFinished;
                if (script is not null && evt is not null)
                {
                    _openAnimation = new UIAnimation(evt);
                    _isPlaying = _openAnimation.PlayAnimation(script, ()=>_OnOpenWindowFinishedCallback(fetus));
                }
            }
            
            if (_isPlaying)
            {
                _playAnimationMask.OpenWindow();
            }
            else
            {
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            _openAnimation?.Dispose();
            _openAnimation = null;

            if (_isPlaying)
            {
                _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
            
            AssertTools.IsTrue(!fetus.isDelayedCloseWindow);
        }

        private void _OnOpenWindowFinishedCallback(WindowFetus fetus)
        {
            _playAnimationMask.CloseWindow();
            _isPlaying = false;

            if (fetus.isDelayedCloseWindow)
            {
                fetus.isDelayedCloseWindow = false;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else
            {
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            fetus.isDelayedCloseWindow = false;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            fetus.isDelayedCloseWindow = true;
        }

        private UIAnimation _openAnimation;
        private bool _isPlaying;
    }
}