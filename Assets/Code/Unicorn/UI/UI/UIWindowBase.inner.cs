/********************************************************************
created:    2022-10-06
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
                        // 2025-07-23
                        // 1. AtLoaded事件, 本来是在handler()之后处理, 但是当使用InitBy(fn)对window进行初始化的时候, 有可能借助AtLoaded配置一些
                        //  window相关的数值, 而这些数值进一步有可能在OnLoaded()回调中使用到, 因此调整为先执行AtLoaded事件, 后执行OnLoaded()回调
                        // 2. AtLoaded, AtOpened, AtClosing, AtUnloading事件在生命周期中只执行一次, 因此执行完成可以合理的清理掉
                        case EventIndices.OnLoaded:
                            AtLoaded?.Invoke();
                            AtLoaded = null;
                            manager.Loaded?.Invoke(this);
                            break;
                        case EventIndices.OnOpened:
                            AtOpened?.Invoke();
                            AtOpened = null;
                            // manager.Opened?.Invoke(this);
                            break;
                        case EventIndices.OnActivated:
                            // Activated?.Invoke();
                            // manager.Activated?.Invoke(this);
                            break;
                        case EventIndices.OnDeactivating:
                            // manager.Deactivating?.Invoke(this);
                            // Deactivating?.Invoke();
                            break;
                        case EventIndices.OnClosing:
                            // manager.Closing?.Invoke(this);
                            AtClosing?.Invoke();
                            AtClosing = null;
                            break;
                        case EventIndices.OnUnloading:
                            manager.Unloading?.Invoke(this);
                            AtUnloading?.Invoke();
                            AtUnloading = null;
                            break;
                    }

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

        internal void ResetDisposed()
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
            // 3. 2025-07-23 这些事件, 在_Handle()方法已经清理过了, 这里算保底
            AtLoaded = null;
            AtOpened = null;
            // Activated = null;
            // Deactivating = null;
            AtClosing = null;
            AtUnloading = null;
            
            // 有些window没有机会AtLoaded, 直接就被Dispose()了. 所以AtUnloading不一定会执行, 但AtDisposing一定会执行
            AtDisposing?.Invoke();
            AtDisposing = null;

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