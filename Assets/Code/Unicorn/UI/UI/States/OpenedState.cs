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
    internal class OpenedState : UIStateBase
    {
        public override void OnEnter(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;

            master.InnerOnOpened("[OnEnter()]");
            UIManager.It._SetForegroundWindow(master, master.GetRenderQueue());
            fetus.AddFlag(FetusFlags.Opened);

            // 如果在OnOpened()或OnActivated()中收到了CloseWindow()
            if (_delayedAction == DelayedAction.CloseWindow)
            {
                _delayedAction = DelayedAction.None;
                fetus.ChangeState(StateKind.CloseAnimation);
            }
        }

        public override void OnExit(WindowFetus fetus, object arg1)
        {
            var master = fetus.master;
            // isOpened is used to judge whether the window can be activated, thus it must be set to be false as soon as OnExit() is called.
            fetus.RemoveFlag(FetusFlags.Opened);

            UIManager.It._OnClosingWindow(master);
            master.InnerOnClosing("[OnExit()]");

            if (_delayedAction == DelayedAction.OpenWindow)
            {
                _delayedAction = DelayedAction.None;
                fetus.ChangeState(StateKind.Opened);
            }
        }

        public override void OnOpenWindow(WindowFetus fetus)
        {
            if (fetus.IsDebugging())
            {
                Logo.Warn("[OpenedState.OnOpenWindow()]");    
            }
            
            if (fetus.HasFlag(FetusFlags.Opened))
            {
                // 之所以要加这一句, 是为了防止前面有代码调用CloseWindow()修改过_nextKind
                fetus.ChangeState(StateKind.Opened);

                // 原代码流程
                var master = fetus.master;
                UIManager.It._SetForegroundWindow(master, master.GetRenderQueue());
            }
            else
            {
                _delayedAction = DelayedAction.OpenWindow;
            }
        }

        public override void OnCloseWindow(WindowFetus fetus)
        {
            if (fetus.HasFlag(FetusFlags.Opened))
            {
                fetus.ChangeState(StateKind.CloseAnimation);
            }
            else
            {
                _delayedAction = DelayedAction.CloseWindow;
            }
            
            if (fetus.IsDebugging())
            {
                Logo.Warn("[ClosedState.OnOpenWindow()]");    
            }
        }

        private DelayedAction _delayedAction; // 遇到了OpenWindow/CloseWindow的请求
    }
}