/********************************************************************
created:    2022-08-16
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
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
            _openAnimation.SetEnabled(false);

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
                Logo.Warn($"[OpenAnimationState.OnOpenWindow()] fetus=[{fetus}]");
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
                Logo.Warn($"[OpenAnimationState.OnCloseWindow()] fetus=[{fetus}]");
            }
        }

        private UIWindowAnimation _openAnimation;
        private bool _isPlaying;
        private DelayedAction _delayedAction; // 遇到了CloseWindow()的请示
    }
}