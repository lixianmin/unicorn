/********************************************************************
created:    2022-10-03
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
#if UNICORN_EDITOR

using System;
using DG.Tweening;
using Unicorn;
using Unicorn.UI;
using UnityEngine;

namespace Scripts
{
    public class TweenAlpha : UIWindowAnimation
    {
        private void OnEnable()
        {
            if (canvasGroup == null)
            {
                Logo.Error("canvasGroup is null");
            }
            
            canvasGroup.DOFade(targetAlpha, duration).OnComplete(() =>
            {
                OnAnimationComplete?.Invoke();
            });
        }
        
        // 为什么不使用public变量? 使用private是为最小化访问权限, 而为了支持序列化就需要补一个SerializeField属性
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private float targetAlpha;

        public override event Action OnAnimationComplete;
    }
}

#endif