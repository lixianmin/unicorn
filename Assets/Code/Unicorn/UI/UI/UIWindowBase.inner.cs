
/********************************************************************
created:    2022-10-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;
using UnityEngine;

namespace Unicorn.UI
{
    public partial class UIWindowBase
    {
        protected UIWindowBase()
        {
            _fetus = new WindowFetus(this);
        }

        internal void InnerOnLoaded() { OnLoaded(); }
        internal void InnerOnOpened() { OnOpened(); }
        internal void InnerOnActivated() { OnActivated(); }
        internal void InnerOnDeactivating() { OnDeactivating(); }
        internal void InnerOnClosing() { OnClosing(); }
        internal void InnerOnUnloading() { OnUnloading(); }
        internal void InnerSlowUpdate(float deltaTime) { SlowUpdate(deltaTime); }
        
        internal void Dispose()
        {
            if (_isReleased)
            {
                return;
            }

            _isReleased = true;
            UIManager.Instance._RemoveWindow(GetType());
            
            _fetus.Dispose();
            _fetus = null;
            _widgets.Clear();
        }

        internal WindowFetus GetFetus() { return _fetus; }

        internal bool Is2D()
        {
            var assetPath = GetAssetPath();
            return assetPath != null && assetPath.Contains("/");
        }
        
        internal void _InitComponents(Transform transform, Canvas canvas)
        {
            _transform = transform;
            _canvas = canvas;
        }

        /// <summary>
        /// canvas.sortingOrder的最大值是32767
        /// </summary>
        /// <param name="order"></param>
        internal void _SetSortingOrder(int order)
        {
            if (_sortingOrder != order)
            {
                _sortingOrder = order;
                
                // canvas需要设置canvas.overrideSorting = true, 并且设置不一样的sortingOrder, 加载出来的按钮才不是灰化的
                // order越大, 越显示在前面
                if (_canvas is not null)
                {
                    // canvas.overrideSorting = true; // 这个在资源加载完成的时候设置
                    _canvas.sortingOrder = order;
                    // Console.WriteLine($"sortingOrder={canvas.sortingOrder}, queue={targetWindow.GetRenderQueue()}, activateVersion={_version}");
                }
            }
        }

        /// <summary>
        /// 出于性能考虑外面只读取, 设置需要走_SetSortingOrder()方法
        /// </summary>
        internal int _sortingOrder;

        private WindowFetus _fetus;
        private Transform _transform;
        private Canvas _canvas;
        private  bool _isReleased;
    }
}