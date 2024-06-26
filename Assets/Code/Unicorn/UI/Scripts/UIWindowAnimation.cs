
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