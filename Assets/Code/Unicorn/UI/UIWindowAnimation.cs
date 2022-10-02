
/********************************************************************
created:    2022-10-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract class UIWindowAnimation : MonoBehaviour
    {
        protected virtual void Start()
        {
            OnAnimationComplete += _OnAnimationDone;
            CoroutineManager.StartCoroutine(_CoAnimationTimeout(), out _animationRoutine);
        }

        private IEnumerator _CoAnimationTimeout()
        {
            const float maxTime = 5.0f;
            var breakTime = Time.time + maxTime;
            while (Time.time < breakTime)
            {
                yield return null;
            }

            _OnAnimationDone();
        }
        
        private void _OnAnimationDone()
        {
            _animationRoutine?.Kill();
            
            _animationRoutine = null;
            OnAnimationComplete -= _OnAnimationDone;
            OnAnimationDone?.Invoke();
        }

        /// <summary>
        /// 无论是OnAnimationComplete还是_CoAnimationTimeout(), 都被会调用OnAnimationDone
        /// </summary>
        internal Action OnAnimationDone;
        
        private CoroutineItem _animationRoutine;
        
        /// <summary>
        /// OnAnimationComplete由子类触发, 基类触发的是_CoAnimationTimeout()
        /// </summary>
        protected abstract event Action OnAnimationComplete;
    }
}