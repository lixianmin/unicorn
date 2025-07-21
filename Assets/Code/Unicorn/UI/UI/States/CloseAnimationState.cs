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
    internal class CloseAnimationState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn($"[CloseAnimationState.OnEnter()] _delayedAction={_delayedAction}");    
            }
            
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

                // _playAnimationMask.OpenWindow();
            }
            else
            {
                fetus.ChangeState(StateKind.Closed);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn($"[CloseAnimationState.OnExit()] _delayedAction={_delayedAction}");    
            }
            
            _closeAnimation = null;

            if (_isPlaying)
            {
                // _playAnimationMask.CloseWindow();
                _isPlaying = false;
            }
        }

        private void _OnCloseWindowAnimationDone(WindowFetus fetus)
        {
            // _playAnimationMask.CloseWindow();
            _isPlaying = false;
            _closeAnimation.SetEnabled(false);

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
            if (fetus.IsDebugging())
            {
                Logo.Warn("[CloseAnimationState.OnOpenWindow()]");    
            }

            if (_isPlaying)
            {
                _delayedAction = DelayedAction.OpenWindow;
            }
            else
            {
                fetus.ChangeState(StateKind.OpenAnimation);
            }
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _delayedAction = DelayedAction.CloseWindow;
            
            if (fetus.IsDebugging())
            {
                Logo.Warn("[CloseAnimationState.OnCloseWindow()]");    
            }
        }

        private UIWindowAnimation _closeAnimation;
        private bool _isPlaying;
        private DelayedAction _delayedAction; // 遇到了OpenWindow()的请求
    }
}