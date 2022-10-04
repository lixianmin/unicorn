/********************************************************************
created:    2022-08-15
author:     lixianmin


 窗体加载生命周期
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
                    window.Update();
                }
            }
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
            var isClosingForeground = GetForegroundWindow() == closingWindow;
            if (isClosingForeground)
            {
                var nextForeground = _SearchNextForeground(closingWindow);
                _SetForegroundWindow(nextForeground);
            }
            else
            {
                _SendDeactivating(closingWindow);
            }
        }

        private static UIWindowBase _SearchNextForeground(UIWindowBase foreground)
        {
            var count = _windowsZOrder.Count;
            var found = false;
            for (var i = count - 1; i >= 0; i--)
            {
                var window = _windowsZOrder[i];
                if (window != foreground)
                {
                    // only window behind the closingWindow can be activated.
                    // only a isOpened window can be activated.
                    if (!window.GetFetus().isOpened)
                    {
                        continue;
                    }
                    
                    return found ? window : null;
                }

                found = true;
            }

            return null;
        }
        
        public static void SetForegroundWindow(UIWindowBase window)
        {
            var fetus = window?.GetFetus();
            if (fetus is not { isOpened: true })
            {
                return;
            }
            
            _SetForegroundWindow(window);
        }
        internal static void _SetForegroundWindow(UIWindowBase window)
        {
            var lastWindow = GetForegroundWindow();
            if (lastWindow != window)
            {
                _SendDeactivating(lastWindow);
                _foregroundWindow = window;
                _SendActivated(window);
            }
        }

        public static UIWindowBase GetForegroundWindow()
        {
            return _foregroundWindow;
        }

        private static void _SendDeactivating(UIWindowBase targetWindow)
        {
            if (targetWindow is not null)
            {
                CallbackTools.Handle(targetWindow.OnDeactivating, "[_SendDeactivating()]");
            }
        }

        private static void _SendActivated(UIWindowBase targetWindow)
        {
            if (targetWindow == null)
            {
                return;
            }

            _version++;
            // todo 直接用_version不行, 最大值是32767
            targetWindow.activateVersion = _version;

            // 本方法中必须调整zorder, 原因是很多时候我们并不关闭窗口, 而是只不断的activate各个窗口, 这时没有load过程
            // todo 但是, 只在这里sort也许是不够的, 原因是如果存在加载动画, 我们会看到新窗口的动画是在background执行的
            _windowsZOrder.Sort((a, b) => a.GetSortingOrder() - b.GetSortingOrder());

            // canvas需要设置canvas.overrideSorting = true, 并且设置不一样的sortingOrder, 加载出来的按钮才不是灰化的
            // order越大, 越显示在前面
            var canvas = targetWindow.GetCanvas();
            if (canvas is not null)
            {
                // canvas.overrideSorting = true; // 这个在资源加载完成的时候设置
                canvas.sortingOrder = targetWindow.GetSortingOrder();
                // Console.WriteLine($"sortingOrder={canvas.sortingOrder}, {targetWindow.GetSortingOrder()}, queue={targetWindow.GetRenderQueue()}, activateVersion={_version}");
            }

            // _SortWindowsTransform();
            CallbackTools.Handle(targetWindow.OnActivated, "[_SendActivated()]");
        }

        // private static void _SortWindowsTransform()
        // {
        //     var count = _windowsZOrder.Count;
        //     if (count <= 1)
        //     {
        //         return;
        //     }
        //
        //     var deltaIndex = 0;
        //     for (var i = 0; i < count; i++)
        //     {
        //         var window = _windowsZOrder[i];
        //         var transform = window.GetTransform();
        //         if (transform is not null)
        //         {
        //             var targetIndex = i - deltaIndex;
        //             var lastIndex = transform.GetSiblingIndex();
        //             if (lastIndex != targetIndex)
        //             {
        //                 transform.SetSiblingIndex(targetIndex);
        //             }
        //             
        //             // Console.WriteLine("transform={0}, lastIndex={1}, targetIndex={2}", transform.name, lastIndex, targetIndex);
        //         }
        //         else
        //         {
        //             deltaIndex++;
        //         }
        //     }
        // }
        
        public static Transform GetUIRoot()
        {
            if (_uiRoot is not null) return _uiRoot;
            
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
        private static int _version;
        private static readonly Snapshot _snapshot = new ();

        private class Snapshot
        {
            public readonly List<UIWindowBase> windows = new(4);
            public int version;
        }
        
        private static UIWindowBase _foregroundWindow;
        private static Transform _uiRoot;
    }
}