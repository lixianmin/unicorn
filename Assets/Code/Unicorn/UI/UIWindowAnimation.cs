
/********************************************************************
created:    2022-10-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract class UIWindowAnimation : MonoBehaviour
    {
        internal void Init(Action onAnimationDone)
        {
            if (onAnimationDone == null)
            {
                return;
            }
            
            _onAnimationDone = onAnimationDone;
            OnAnimationComplete += _OnAnimationDone;
            
            const float delayedSeconds = 5f;
            Loom.RunDelayed(_OnAnimationDone, delayedSeconds);
        }

        private void _OnAnimationDone()
        {
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