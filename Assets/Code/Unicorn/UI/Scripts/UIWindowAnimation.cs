
/********************************************************************
created:    2022-10-02
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

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unicorn.UI
{
    /// <summary>
    /// 1. 在Assets/link.xml中有Unicorn.dll + stripping level=Low, 但在 ios+hybrid clr的过程中仍然被裁剪掉了
    /// 2. 目前在client项目的Assets/link.xml中加入本类型是好使的
    /// 3. 尝试添加 [Preserve] 属性, 希望对改善这件事有帮助, 但未测试, 但相信不会变得更差
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public abstract class UIWindowAnimation : MonoBehaviour
    {
        public async Task Init(Action onAnimationDone)
        {
            if (onAnimationDone == null)
            {
                return;
            }

            _isHandled = false;
            _onAnimationDoneCallback = onAnimationDone;
            OnAnimationComplete += _OnAnimationDone;
            
            await Task.Delay(TimeSpan.FromSeconds(_timeout));
            _OnAnimationDone();
        }

        private void _OnAnimationDone()
        {
            // Logo.Info($"threadId={Thread.CurrentThread.ManagedThreadId}, _isHandled={_isHandled}");
            if (!_isHandled)
            {
                _isHandled = true;
                // OnAnimationComplete -= new Action(_OnAnimationDone); // 与下面这一句等价
                OnAnimationComplete -= _OnAnimationDone; // 这里事件加减其实使用的不是同一个Action对象，但是没关系，这种操作是正确的
                _onAnimationDoneCallback?.Invoke();
            }
        }

        /// <summary>
        /// 动画超时会被强制中断，单位（秒）
        /// </summary>
        [SerializeField] private float _timeout = 3f;
        
        /// <summary>
        /// 无论是OnAnimationComplete还是_CoAnimationTimeout(), 都被会调用OnAnimationDone
        /// </summary>
        private Action _onAnimationDoneCallback;

        private bool _isHandled;

        /// <summary>
        /// OnAnimationComplete由子类触发, 设计为public则是希望外部可以访问到
        /// </summary>
        public abstract event Action OnAnimationComplete;
    }
}