/********************************************************************
created:    2022-10-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

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