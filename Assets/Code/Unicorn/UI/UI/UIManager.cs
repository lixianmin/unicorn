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
using Unicorn.UI.Internal;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

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
    public class UIManager
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
        public UIWindowBase OpenWindow(Type windowType)
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
        /// 1. 按指定windowType关闭一个window。如果被关闭的window是前台窗体，则它后面的那个window会收到OnActivated()事件.
        /// 2. 调用CloseWindow()后, 并不意味着这个窗体立即销毁了, 需要等待关闭的状态机流程执行完成
        /// </summary>
        /// <param name="windowType"></param>
        public void CloseWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            item.window?.GetFetus().CloseWindow();
        }

        internal void _RemoveWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            if (item.window != null)
            {
                _windowsZOrder.RemoveAt(item.order);
                _version++;
            }
        }

        public UIWindowBase GetWindow(Type windowType)
        {
            var item = _IndexWindow(windowType);
            return item.window;
        }

        public IEnumerable<UIWindowBase> EnumerateWindows()
        {
            return _windowsZOrder;
        }

        internal void SlowUpdate(float deltaTime)
        {
            var snapshot = _TakeSnapshot();
            var windows = snapshot.windows;
            var count = windows.Count;

            // 有些场景并没有UI, 也找不到UICamera, 如果任由_CheckUICameraChanged()调用, 会大量报错
            if (count > 0)
            {
                if (_uiCameraName != string.Empty && _uiCamera == null)
                {
                    _CheckUICameraChanged(windows);
                }

                for (var i = 0; i < count; i++)
                {
                    var window = windows[i];
                    if (window.GetFetus().HasFlag(FetusFlags.Loaded))
                    {
                        window.InnerSlowUpdate(deltaTime);
                    }
                }
            }
        }

        /// <summary>
        /// 在切换场景的时候UICamera可能会重建, 这会导致原先的ui中对UICamera的引用消失
        /// </summary>
        /// <param name="windows"></param>
        private void _CheckUICameraChanged(List<UIWindowBase> windows)
        {
            var nextCamera = GetUICamera();
            if (nextCamera == null)
            {
                return;
            }

            var position = nextCamera.transform.position;
            if (position != Vector3.zero)
            {
                nextCamera.transform.position = Vector3.zero;
                if (Application.isEditor)
                {
                    Logo.Warn($"UICamera is at {position}, it will be forced set at (0, 0, 0)");
                }
            }

            var count = windows.Count;
            for (var i = 0; i < count; i++)
            {
                var window = windows[i];
                var canvas = window.GetCanvas();
                SetCanvasWorldCamera(canvas);
            }
        }

        internal void SetCanvasWorldCamera(Canvas canvas)
        {
            if (canvas != null)
            {
                switch (canvas.renderMode)
                {
                    case RenderMode.ScreenSpaceOverlay:
                        canvas.worldCamera = null;
                        break;
                    case RenderMode.ScreenSpaceCamera:
                        canvas.worldCamera = GetUICamera();
                        break;
                    case RenderMode.WorldSpace:
                        canvas.worldCamera = Camera.main;
                        break;
                }
            }
        }

        internal void ExpensiveUpdate(float deltaTime)
        {
            var snapshot = _TakeSnapshot();
            var windows = snapshot.windows;
            var count = windows.Count;
            for (var i = 0; i < count; i++)
            {
                var window = windows[i];
                var fetus = window.GetFetus();
                fetus.ExpensiveUpdate();

                if (fetus.HasFlag(FetusFlags.Loaded))
                {
                    var updater = window as IExpensiveUpdater;
                    updater?.ExpensiveUpdate(deltaTime);
                }
            }

            _CheckResetSortingOrders();
        }

        private UIWindowBase _FetchWindow(Type windowType)
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

        private WindowItem _IndexWindow(Type windowType)
        {
            if (windowType != null)
            {
                var count = _windowsZOrder.Count;
                for (var order = count - 1; order >= 0; order--)
                {
                    var window = _windowsZOrder[order];
                    if (window.GetType() == windowType)
                    {
                        return new WindowItem { order = order, window = window };
                    }
                }
            }

            return new WindowItem { order = -1, window = null };
        }

        internal void _OnClosingWindow(UIWindowBase closingWindow)
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

        private UIWindowBase _SearchNextForeground(UIWindowBase foreground)
        {
            if (foreground != null)
            {
                var foregroundQueue = foreground.GetRenderQueue();
                var foregroundOrder = foreground._sortingOrder;

                var count = _windowsZOrder.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    var window = _windowsZOrder[i];
                    if (window._sortingOrder < foregroundOrder && window.GetFetus().HasFlag(FetusFlags.Opened))
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
        /// 这个方法应该直接使用OpenWindow(typeof(XXX))代替
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
        internal void _SetForegroundWindow(UIWindowBase window, RenderQueue queue)
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

        public UIWindowBase GetForegroundWindow(RenderQueue queue)
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

        private void _SendDeactivating(UIWindowBase targetWindow)
        {
            targetWindow?.InnerOnDeactivating("[_SendDeactivating()]");
        }

        private void _SendActivated(UIWindowBase targetWindow)
        {
            if (targetWindow is not null)
            {
                _ActivateWindow(targetWindow);
                targetWindow.InnerOnActivated("[_SendActivated()]");
            }
        }

        internal void _ActivateWindow(UIWindowBase targetWindow)
        {
            // AssertTools.IsNotNull(targetWindow);
            _version++;

            targetWindow._SetSortingOrder((int)targetWindow.GetRenderQueue() + _maxZOrder++);

            // 本方法中必须调整zorder, 原因是很多时候我们并不关闭窗口, 而是只不断的activate各个窗口, 这时没有load过程
            // 但是, 只在这里sort也许是不够的, 原因是如果存在加载动画, 我们会看到新窗口的动画是在background执行的
            _windowsZOrder.Sort((a, b) => a._sortingOrder - b._sortingOrder);

            // Logo.Info($"targetWindow={targetWindow.GetType()}, windowsZOrder={",".JoinEx(_windowsZOrder, item => item._GetSortingOrder().ToString())}, targetOrder={targetWindow._GetSortingOrder()}");
        }

        /// <summary>
        /// 每隔一段时间重置一次sortingOrder
        /// </summary>
        private void _CheckResetSortingOrders()
        {
            if (Time.time > _nextResetZOrderTime)
            {
                _nextResetZOrderTime = Time.time + 5f;

                const int step = (int)RenderQueue.Geometry - (int)RenderQueue.Background;
                var count = _windowsZOrder.Count;
                for (int i = 0; i < count; i++)
                {
                    var window = _windowsZOrder[i];
                    var lastSortingOrder = window._sortingOrder;
                    var nextSortingOrder = lastSortingOrder - lastSortingOrder % step + i;

                    window._SetSortingOrder(nextSortingOrder);
                }

                _maxZOrder = count;
                // Logo.Info($"windowsZOrder={",".JoinEx(_windowsZOrder, item => item.sortingOrder.ToString())}, _maxZOrder={_maxZOrder}");
            }
        }

        public Transform GetUIRoot()
        {
            if (_uiRoot is not null)
            {
                return _uiRoot;
            }

            const string name = "UIRoot";
            var goRoot = GameObject.Find(name);
            if (goRoot == null)
            {
                goRoot = new GameObject(name);
            }

            // DontDestroyOnLoad()只能在play mode下调用，否则会报InvalidOperationException
            // 保证切换场景时UIRoot不会被莫名其妙的销毁
            if (Application.isPlaying)
            {
                Object.DontDestroyOnLoad(goRoot);
            }

            _uiRoot = goRoot.transform;
            return _uiRoot;
        }

        public void SetUICameraName(string name)
        {
            _uiCameraName = name ?? string.Empty;
        }

        internal Camera GetUICamera()
        {
            if (_uiCamera == null)
            {
                if (string.IsNullOrEmpty(_uiCameraName))
                {
                    Logo.Error("UICamera name is not set");
                    return null;
                }

                var gameObject = GameObject.Find(_uiCameraName);
                if (null == gameObject)
                {
                    Logo.Error("Can not find UICamera with name={0}", _uiCameraName);
                    return null;
                }

                _uiCamera = gameObject.GetComponent<Camera>();
                // 原设计中, UICamera放在Camera节点下 (Main Camera, CM vcam1等)
                // 但, 由于BehaviorDesigner镜头动画的需要, 每个场景都需要单独配置自己的Main Camera, 所以整个Camera节点也转移到了各场景
                // 但, 这带来的问题是: UICamera每个场景也有了多个, 这导致已经设置的UICamera的ui在切换场景后对UICamera的引用消失了
                // ~~所以, 决定把UICamera单独摘出来, 并且设置为 DontDestroyOnLoad()~~
                //
                // 上面这个方案不行, 因为MainCamera上面要设置Stack, 只能是在同场景中设置. 改为在SlowUpdate()中搞一搞
                // Object.DontDestroyOnLoad(gameObject);
            }

            return _uiCamera;
        }

        private Snapshot _TakeSnapshot()
        {
            if (_version != _snapshot.version)
            {
                _snapshot.windows.Clear();
                _snapshot.windows.AddRange(_windowsZOrder);
                _snapshot.version = _version;
            }

            return _snapshot;
        }

        /// <summary>
        /// 设计为singleton而不是static类，是为了给未来的自己一个机会：万一client端需要重写一些方法呢？
        /// </summary>
        public static readonly UIManager It = new();

        public Action<UIWindowBase> Loaded;
        // public Action<UIWindowBase> Opened;
        // public Action<UIWindowBase> Activated;
        // public Action<UIWindowBase> Deactivating;
        // public Action<UIWindowBase> Closing;
        public Action<UIWindowBase> Unloading;

        private readonly List<UIWindowBase> _windowsZOrder = new(4);
        private float _nextResetZOrderTime;
        private int _maxZOrder;

        private int _version;
        private readonly Snapshot _snapshot = new();

        private class Snapshot
        {
            public readonly List<UIWindowBase> windows = new(4);
            public int version;
        }

        private readonly UIWindowBase[] _foregroundWindows = new UIWindowBase[5]; // 第0个位置必然一直是null
        private Transform _uiRoot;

        // 曾经的默认行为是canvas.renderMode = ScreenSpaceCamera, 也就是使用两个Camera, 一个用于UI, 一个用于WorldSpace
        // 现在, 默认行为改成canvas.renderMode = ScreenSpaceOverlay, 也就是不使用UICamera, 这样可以节约一遍绘制
        private Camera _uiCamera; // 用于canvas.renderMode = ScreenSpaceCamera
        private string _uiCameraName = string.Empty; // 默认是空, 也就是不使用UICamera
    }
}