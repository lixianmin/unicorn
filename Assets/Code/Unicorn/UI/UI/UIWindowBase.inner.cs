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
        private enum EventIndices
        {
            OnLoaded = 0,
            OnOpened = 1,
            OnActivated = 2,
            OnDeactivating = 3,
            OnClosing = 4,
            OnUnloading = 5,
        }

        protected UIWindowBase()
        {
            _fetus = new WindowFetus(this);
        }

        internal void InnerOnLoaded(string title)
        {
            _Handle(OnLoaded, title, EventIndices.OnLoaded);
        }

        internal void InnerOnOpened(string title)
        {
            _Handle(OnOpened, title, EventIndices.OnOpened);
        }

        internal void InnerOnActivated(string title)
        {
            _Handle(OnActivated, title, EventIndices.OnActivated);
        }

        internal void InnerOnDeactivating(string title)
        {
            _Handle(OnDeactivating, title, EventIndices.OnDeactivating);
        }

        internal void InnerOnClosing(string title)
        {
            _Handle(OnClosing, title, EventIndices.OnClosing);
        }

        internal void InnerOnUnloading(string title)
        {
            _Handle(OnUnloading, title, EventIndices.OnUnloading);
        }

        internal void InnerSlowUpdate(float deltaTime)
        {
            SlowUpdate(deltaTime);
        }

        private void _Handle(Action handler, string title, EventIndices idx)
        {
            var index = (int)idx;
            if (((_ongoings >> index) & 1) == 0)
            {
                _ongoings |= (byte)(1 << index);
                var manager = UIManager.It;
                try
                {
                    switch (idx)
                    {
                        case EventIndices.OnDeactivating:
                            // manager.Deactivating?.Invoke(this);
                            // Deactivating?.Invoke();
                            break;
                        case EventIndices.OnClosing:
                            // manager.Closing?.Invoke(this);
                            Closing?.Invoke();
                            break;
                        case EventIndices.OnUnloading:
                            manager.Unloading?.Invoke(this);
                            Unloading?.Invoke();
                            break;
                    }

                    handler();

                    switch (idx)
                    {
                        case EventIndices.OnLoaded:
                            Loaded?.Invoke();
                            manager.Loaded?.Invoke(this);
                            break;
                        case EventIndices.OnOpened:
                            Opened?.Invoke();
                            // manager.Opened?.Invoke(this);
                            break;
                        case EventIndices.OnActivated:
                            // Activated?.Invoke();
                            // manager.Activated?.Invoke(this);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logo.Error("[_Handle()] {0}, ex= {1},\n\n StackTrace={2}", title, ex, ex.StackTrace);
                }

                _ongoings &= (byte)~(1 << index);
                // AssertTools.IsTrue(((_ongoings >> index) & 1) == 0);
            }
        }

        internal void ResetDispose()
        {
            _isDisposed = false;
        }
        
        internal void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            UIManager.It._RemoveWindow(GetType());
            
            // 1. 清理所有事件回调 (即使是被Cache, 也要清理Loaded, Unloading这些回调事件, 否则再次激活的时候, 很可能会被重复加入两次同样的回调方法)
            // 2. 在这个代码位置全部设置为null作为兜底逻辑是合适的, 因为代码执行到这里, 意味着window从逻辑上已经Dispose了 (Cache算优化, 不影响逻辑)
            // 3. 因为Loaded, Opened, Closing, Unloading在window生命周期只执行一次, 因此在OnLoaded(), OnOpened() 中用 Unloading += button.onClick(fn)
            //  是合理的, 在window重新被OpenWindow()的时候也不会重复注册同一个事件多次. 但是, 在OnActivated()中注册 Deactivating += button.onClick(fn)
            //  可能是不合理的, 除非程序员记得在 OnDeactivating()中手动反注册这些回调方法
            // 4. 为了防止程序员出错, 先移除Activated, Deactivating事件, 也是一个选择, 毕竟对它们的使用本就罕见.
            Loaded = null;
            Opened = null;
            // Activated = null;
            // Deactivating = null;
            Closing = null;
            Unloading = null;

            var flags = GetWindowFlags();
            var needCache = flags.HasFlag(WindowFlags.Cache);
            if (needCache)
            {
                WindowCache.It.AddWindow(this);
                return;
            }

            _fetus.Dispose();
            _fetus = null;
            _transform = null;
            _canvas = null;

            _RemoveWidgetListeners();
        }

        internal WindowFetus GetFetus()
        {
            return _fetus;
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
                // if (_canvas is not null)
                if (_canvas != null)
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
        private bool _isDisposed;
        private byte _ongoings; // 防止回调方法递归调用自己
    }
}