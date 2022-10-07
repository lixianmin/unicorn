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
                _openAnimation = serializer.openWindowAnimation;
                if (_openAnimation is not null)
                {
                    _openAnimation.enabled = true;
                    _=_openAnimation.Init(() =>
                    {
                        _OnOpenWindowAnimationDone(fetus);
                    });

                    _isPlaying = true;
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
            _openAnimation = null;

            if (_isPlaying)
            {
                _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
            
            AssertTools.IsTrue(!fetus.isDelayedCloseWindow);
        }

        private void _OnOpenWindowAnimationDone(WindowFetus fetus)
        {
            _playAnimationMask.CloseWindow();
            _isPlaying = false;

            if (_openAnimation is not null)
            {
                _openAnimation.enabled = false;
            }

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

        private UIWindowAnimation _openAnimation;
        private bool _isPlaying;
    }
}