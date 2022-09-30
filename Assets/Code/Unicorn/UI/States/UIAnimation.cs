/********************************************************************
created:    2022-08-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using Unicorn.UI.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Unicorn.UI.States
{
    internal class UIAnimation
    {
        public UIAnimation(UnityEvent onFinished)
        {
            if (onFinished is null)
            {
                return;
            }

            _onFinished = onFinished;
            
            _onFinishedHandler = () =>
            {
                _StopAnimationRoutine();
                _onFinishedCallback?.Invoke();
            };
                
            _onFinished.AddListener(_onFinishedHandler);
        }

        public void Dispose()
        {
            if (_onFinishedHandler is not null)
            {
                _onFinished.RemoveListener(_onFinishedHandler);
                _onFinishedHandler = null;
            }
            
            _StopAnimationRoutine();
            _onFinishedCallback = null;
        }

        public bool PlayAnimation(MonoBehaviour animationScript, Action onFinishedCallback)
        {
            if (animationScript is null || onFinishedCallback is null)
            {
                return false;
            }

            _onFinishedCallback = onFinishedCallback;
            _StopAnimationRoutine();
            
            animationScript.enabled = true;
            animationScript.enabled = false;
            CoroutineManager.StartCoroutine(_CoWaitAnimationDone(), out _animationRoutine);
            
            return true;
        }

        private IEnumerator _CoWaitAnimationDone()
        {
            const float maxTime = 2.0f;
            var breakTime = Time.time + maxTime;
            while (Time.time < breakTime)
            {
                yield return null;
            }

            _animationRoutine = null;
            _onFinishedCallback?.Invoke();
        }

        private void _StopAnimationRoutine()
        {
            _animationRoutine?.Kill();
        }
        
        private UnityEvent _onFinished;

        private CoroutineItem _animationRoutine;
        private UnityAction _onFinishedHandler;
        private Action _onFinishedCallback;
    }
}