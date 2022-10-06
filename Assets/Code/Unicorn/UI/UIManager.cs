/********************************************************************
created:    2022-08-15
author:     lixianmin

设计目标:
    1. UI的prefab资源与client代码分离, 减少美术与程序的工作耦合
    2. 通过UISerializer将client代码中需要使用的控件序列化, 对包含大量控件的window可有效减少加载时间
    3. 将所有windows分为4个RenderQueue: Background, Geometry, Transparent, Overlay
    4. 核心方法只有2个: UIManager.OpenWindow(typeof(UIBag)) 和 UIManager.CloseWindow(typeof(UIBag))
    5. 打开和关闭window支持open/close动画
    6. 严格把控生命周期相关回调事件: 严格对称调用 & protected控制访问权限
    7. 尽可能自动化RemoveAllListeners()

 window加载生命周期:
 ----------------------------------------------------------------------
 |  new     --> load   --> open animation  --> open  --> active  -->  |
 |                                                                    |  
 |  dispose <-- unload <-- close animation <-- close <-- deactive <-- |
 ----------------------------------------------------------------------

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unicorn.UI
{
    /// <summary>
    /// 2022-08-26 Update:
    /// 1. 也许可以统一UI制作方式:
    ///     1. 每一个prefab都先创建一个自己的Canvas, 然后在自己的Canvas下面再挂接具体的UI控件
    ///     2. 同时保留UIRoot为空节点
    /// 2. 如果是普通UI, 就先: 右键->UI->Canvas
    /// 3. 如果是XR, 则选: 右键->XR->XR Canvas
    /// 4. 不清楚这个方案的运行性能如何, 不过看起来比较统一, 值得测试. 特别是, 如果是VR同时希望制作world space的UI的话, 好像只能这么做?
    ///  
    ///
    /// 不同的 Canvas.renderMode 要求不同的UI制作方式:
    /// 1. 如果是 RenderMode.ScreenSpaceOverlay 或 RenderMode.ScreenSpaceCamera , 则可以把Canvas, Canvas Scaler, Graphic Raycaster放到UIRoot上
    /// 2. 如果是 RenderMode.WorldSpace, 则需要把Canvas, Canvas Scaler, Graphic Raycaster跟具体的UI放到一起. 原因是WorldSpace中的UI需要位置信息, 这一点好像只能由Canvas对象所在的RectTransform提供
    /// </summary>
    public static class UIManager
    {
        private struct WindowItem
        {
            public int order;
            public UIWindowBase window;
        }
        
        /// <summary>
        /// 打开一个window. 如果window未加载, 则会先执行资源加载过程; 如果window只是在后台，则会激活window到前台
        /// </summary>
        /// <param name="windowType"></param>
        /// <returns></returns>
        public static UIWindowBase OpenWindow(Type windowType)
        {
            if (windowType == null || !windowType.IsSubclassOf(typeof(UIWindowBase)))
            {
                return null;
            }
            
            var window = _FetchWindow(windowType);
            if (window == null)
            {
                return null;
            }
            
            window.GetFetus().OpenWindow();
            return window;
        }
        
        /// <summary>
        /// 按指定windowType关闭一个window。如果被关闭的window是前台窗体，则它后面的那个window会收到OnActivated()事件
        /// </summary>
        /// <param name="windowType"></param>
        public static void CloseWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            item.window?.GetFetus().CloseWindow();
        }

        internal static void _RemoveWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            if (item.window != null)
            {
                _windowsZOrder.RemoveAt(item.order);
                _version++;
            }
        }

        public static UIWindowBase GetWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            return item.window;
        }
        
        public static T GetWindow<T>() where T: UIWindowBase
        {
            return GetWindow(typeof(T)) as T;
        }
        
        // 目前没有必须使用LogicUpdate的需求, 因此先注释掉, 回头有实际需求再打开
        // public static void LogicUpdate(float deltaTime)
        // {
        //     var snapshot = _TakeSnapshot();
        //     var windows = snapshot.windows;
        //     var count = windows.Count;
        //     for (var i = 0; i < count; i++)
        //     {
        //         var window = windows[i];
        //         if (window.GetFetus().isLoaded)
        //         {
        //             window.LogicUpdate(deltaTime);
        //         }
        //     }
        // }
        
        internal static void Update()
        {
            var snapshot = _TakeSnapshot();
            var windows = snapshot.windows;
            var count = windows.Count;
            for (var i = 0; i < count; i++)
            {
                var window = windows[i];
                if (window.GetFetus().isLoaded)
                {
                    window.InnerUpdate();
                }
            }
            
            _CheckResetSortingOrders();
        }
        
        private static UIWindowBase _FetchWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            if (item.window != null)
            {
                return item.window;
            }
            
            if (Activator.CreateInstance(windowType) is not UIWindowBase window)
            {
                throw new NullReferenceException("invalid windowType");
            }

            _windowsZOrder.Add(window);
            _version++;
                
            return window;
        }

        private static WindowItem _IndexWindow(Type windowType)
        {
            if (windowType != null)
            {
                var count = _windowsZOrder.Count;
                for (var order = count - 1; order >= 0; order--)
                {
                    var window = _windowsZOrder[order];
                    if (window.GetType() == windowType)
                    {
                        return new WindowItem{order = order, window = window};
                    }
                }
            }
            
            return new WindowItem{order = -1, window = null};
        }

        internal static void _OnClosingWindow(UIWindowBase closingWindow)
        {
            if (closingWindow != null)
            {
                var queue = closingWindow.GetRenderQueue();
                var isClosingForeground = GetForegroundWindow(queue) == closingWindow;
                if (isClosingForeground)
                {
                    var nextForeground = _SearchNextForeground(closingWindow);
                    _SetForegroundWindow(nextForeground, queue);
                }
                else
                {
                    _SendDeactivating(closingWindow);
                }    
            }
        }

        private static UIWindowBase _SearchNextForeground(UIWindowBase foreground)
        {
            if (foreground != null)
            {
                var foregroundQueue = foreground.GetRenderQueue();
                var foregroundOrder = foreground._sortingOrder;
            
                var count = _windowsZOrder.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    var window = _windowsZOrder[i];
                    if (window._sortingOrder < foregroundOrder && window.GetFetus().isOpened)
                    {
                        return window;
                    }

                    if (window.GetRenderQueue() < foregroundQueue)
                    {
                        return null;
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 这个方法应该可以直接使用OpenWindow(typeof(XXX))代替
        /// </summary>
        /// <param name="window"></param>
        // public static void SetForegroundWindow(UIWindowBase window)
        // {
        //     var fetus = window?.GetFetus();
        //     if (fetus is not { isOpened: true })
        //     {
        //         return;
        //     }
        //     
        //     _SetForegroundWindow(window);
        // }
        
        /// <summary>
        /// window有可能是null, 所以需要指定是哪一个queue
        /// </summary>
        /// <param name="window"></param>
        /// <param name="queue"></param>
        internal static void _SetForegroundWindow(UIWindowBase window, RenderQueue queue)
        {
            var index = _GetForegroundWindowIndex(queue);
            var lastWindow = _foregroundWindows[index];

            if (lastWindow != window)
            {
                _SendDeactivating(lastWindow);
                _foregroundWindows[index] = window;
                _SendActivated(window);
            }
        }

        public static UIWindowBase GetForegroundWindow(RenderQueue queue)
        {
            var index = _GetForegroundWindowIndex(queue);
            return _foregroundWindows[index];
        }

        private static int _GetForegroundWindowIndex(RenderQueue queue)
        {
            return queue switch
            {
                RenderQueue.Background => 1,
                RenderQueue.Geometry => 2,
                RenderQueue.Transparent => 3,
                RenderQueue.Overlay => 4,
                _ => 0 // 第0个位置必然一直是null
            };
        }

        private static void _SendDeactivating(UIWindowBase targetWindow)
        {
            if (targetWindow is not null)
            {
                CallbackTools.Handle(targetWindow.InnerOnDeactivating, "[_SendDeactivating()]");
            }
        }

        private static void _SendActivated(UIWindowBase targetWindow)
        {
            if (targetWindow is not null)
            {
                _ActivateWindow(targetWindow);
                CallbackTools.Handle(targetWindow.InnerOnActivated, "[_SendActivated()]");
            }
        }

        internal static void _ActivateWindow(UIWindowBase targetWindow)
        {
            AssertTools.IsNotNull(targetWindow);
            _version++;

            targetWindow._SetSortingOrder((int) targetWindow.GetRenderQueue() + _maxZOrder++);

            // 本方法中必须调整zorder, 原因是很多时候我们并不关闭窗口, 而是只不断的activate各个窗口, 这时没有load过程
            // 但是, 只在这里sort也许是不够的, 原因是如果存在加载动画, 我们会看到新窗口的动画是在background执行的
            _windowsZOrder.InsertSortEx((a, b) => a._sortingOrder - b._sortingOrder);

            // Console.WriteLine($"targetWindow={targetWindow.GetType()}, windowsZOrder={",".JoinEx(_windowsZOrder, item => item._GetSortingOrder().ToString())}, targetOrder={targetWindow._GetSortingOrder()}");
        }
        
        /// <summary>
        /// 每隔一段时间重置一次sortingOrder
        /// </summary>
        private static void _CheckResetSortingOrders()
        {
            if (Time.time > _nextResetZOrderTime)
            {
                _nextResetZOrderTime = Time.time + 5f;

                const int step = (int) RenderQueue.Geometry - (int) RenderQueue.Background;
                var count = _windowsZOrder.Count;
                for (int i = 0; i < count; i++)
                {
                    var window = _windowsZOrder[i];
                    var lastSortingOrder = window._sortingOrder;
                    var nextSortingOrder = lastSortingOrder - lastSortingOrder % step + i;
                    
                    window._SetSortingOrder(nextSortingOrder);
                }
                
                _maxZOrder = count;
                // Console.WriteLine($"windowsZOrder={",".JoinEx(_windowsZOrder, item => item.sortingOrder.ToString())}, _maxZOrder={_maxZOrder}");
            } 
        }

        public static Transform GetUIRoot()
        {
            if (_uiRoot is not null)
            {
                return _uiRoot;
            }
            
            var goRoot = GameObject.Find("UIRoot");
            if (goRoot == null)
            {
                throw new NullReferenceException("can not find UIRoot");
            }
            
            #if !UNITY_EDITOR
            GameObject.DontDestroyOnLoad(goRoot);
            #endif
            
            _uiRoot = goRoot.transform;
            return _uiRoot;
        }

        private static Snapshot _TakeSnapshot()
        {
            if (_version != _snapshot.version)
            {
                _snapshot.windows.Clear();
                _snapshot.windows.AddRange(_windowsZOrder);
                _snapshot.version = _version;
            }

            return _snapshot;
        }

        private static readonly List<UIWindowBase> _windowsZOrder = new(4);
        private static float _nextResetZOrderTime;
        private static int _maxZOrder;
        
        private static int _version;
        private static readonly Snapshot _snapshot = new ();

        private class Snapshot
        {
            public readonly List<UIWindowBase> windows = new(4);
            public int version;
        }
        
        private static UIWindowBase[] _foregroundWindows = new UIWindowBase[5]; // 第0个位置必然一直是null
        private static Transform _uiRoot;
    }
}