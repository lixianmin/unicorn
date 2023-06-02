
/********************************************************************
created:    2022-10-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
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

        internal void InnerOnLoaded(string title) { _Handle(OnLoaded, title, 0); }
        internal void InnerOnOpened(string title) { _Handle(OnOpened, title, 1); }
        internal void InnerOnActivated(string title) { _Handle(OnActivated, title, 2); }
        internal void InnerOnDeactivating(string title) { _Handle(OnDeactivating, title, 3); }
        internal void InnerOnClosing(string title) { _Handle(OnClosing, title, 4); }
        internal void InnerOnUnloading(string title) { _Handle(OnUnloading, title, 5); }
        internal void InnerSlowUpdate(float deltaTime) { SlowUpdate(deltaTime); }

        private void _Handle(Action handler, string title, int index)
        {
            if (((_ongoings >> index) & 1) == 0)
            {
                _ongoings |= (byte)(1 << index);
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Logo.Error("[_Handle()] {0}, ex= {1},\n\n StackTrace={2}", title, ex, ex.StackTrace);
                }

                _ongoings &= (byte)~(1 << index);
                // AssertTools.IsTrue(((_ongoings >> index) & 1) == 0);
            }
        }
        
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
            _transform = null;
            _canvas = null;
            RemoveWidgetListeners();
        }

        private void RemoveWidgetListeners()
        {
            if (_widgets.Count > 0)
            {
                foreach (var widget in _widgets.Values)
                {
                    if (widget is IRemoveAllListeners item)
                    {
                        item.RemoveAllListeners();
                    }
                }
                _widgets.Clear();
            }
        }

        internal WindowFetus GetFetus() { return _fetus; }

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
                    // Logo.Info($"sortingOrder={canvas.sortingOrder}, queue={targetWindow.GetRenderQueue()}, activateVersion={_version}");
                }
            }
        }

        /// <summary>
        /// 出于性能考虑外面只读取, 设置需要走_SetSortingOrder()方法
        /// </summary>
        internal int _sortingOrder;

        /// <summary>
        /// 默认是2D的界面，当有canvas时通过canvas.renderMode != WorldSpace判断是否是2D界面，因为3D的界面必须设置空间位置和scale，而这必须在renderMode是WorldSpace的情况下才能调整
        /// </summary>
        internal bool _is2D = true;

        private WindowFetus _fetus;
        private Transform _transform;
        private Canvas _canvas;
        private bool _isReleased;
        private byte _ongoings;    // 防止回调方法递归调用自己
    }
}