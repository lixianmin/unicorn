
/********************************************************************
created:    2022-10-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract class UIWindowAnimation : MonoBehaviour
    {
        internal async Task Init(Action onAnimationDone)
        {
            if (onAnimationDone == null)
            {
                return;
            }
            
            _onAnimationDone = onAnimationDone;
            OnAnimationComplete += _OnAnimationDone;
            
            // 延迟5秒调用一次_OnAnimationDone()方法
            const float delayedSeconds = 5f;
            await Task.Delay(TimeSpan.FromSeconds(delayedSeconds));
            _OnAnimationDone();
        }

        private void _OnAnimationDone()
        {
            // Console.WriteLine($"threadId={Thread.CurrentThread.ManagedThreadId}, _isHandled={_isHandled}");
            if (!_isHandled)
            {
                _isHandled = true;
                OnAnimationComplete -= _OnAnimationDone;
                _onAnimationDone?.Invoke();
            }
        }

        /// <summary>
        /// 无论是OnAnimationComplete还是_CoAnimationTimeout(), 都被会调用OnAnimationDone
        /// </summary>
        private Action _onAnimationDone;

        private bool _isHandled;

        /// <summary>
        /// OnAnimationComplete由子类触发, 基类触发的是_CoAnimationTimeout()
        /// </summary>
        protected abstract event Action OnAnimationComplete;
    }
}