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
            if (fetus.isDelayedOpenWindow)
            {
                fetus.isDelayedOpenWindow = false;
                fetus.ChangeState(StateKind.Opened);
                return;
            }

            var serializer = fetus.GetSerializer();
            if (serializer is not null)
            {
                _closeAnimation = serializer.closeWindowAnimation;
                if (_closeAnimation is not null)
                {
                    _closeAnimation.enabled = true;
                    _=_closeAnimation.Init(() =>
                    {
                        _OnCloseWindowAnimationDone(fetus);
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

            if (_closeAnimation is not null)
            {
                _closeAnimation.enabled = false;
            }

            if (fetus.isDelayedOpenWindow)
            {
                fetus.isDelayedOpenWindow = false;
                fetus.ChangeState(StateKind.OpenAnimation);
            }
            else
            {
                fetus.ChangeState(StateKind.Closed);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            fetus.isDelayedOpenWindow = true;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            fetus.isDelayedOpenWindow = false;
        }

        private UIWindowAnimation _closeAnimation;
        private bool _isPlaying;
    }
}