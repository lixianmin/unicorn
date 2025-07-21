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
    internal class UnloadState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;
            master.InnerOnUnloading("[OnEnterUnloadState()]");

            var isDelayedOpening = _delayedAction == DelayedAction.OpenWindow;
            fetus.RemoveFlag(FetusFlags.Loaded);
            fetus.ChangeState(StateKind.None);
            master.Dispose(); // 这个会设置_isDelayedOpening=false;

            // 如果在关闭的过程中遇到了打开本window的请示，则在关闭后重新打开自己
            if (isDelayedOpening)
            {
                UIManager.It.OpenWindow(master.GetType());
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn("[UnloadState.OnOpenWindow()]");    
            }
            
            _delayedAction = DelayedAction.OpenWindow;
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            _delayedAction = DelayedAction.CloseWindow;
            
            if (fetus.IsDebugging())
            {
                Logo.Warn("[UnloadState.OnCloseWindow()]");    
            }
        }

        private DelayedAction _delayedAction; // 遇到了OpenWindow()的请求
    }
}