
/********************************************************************
created:    2022-08-15
author:     lixianmin

1. life cycle of a window:

 ----------------------------------------------------------------------
 |  new     --> load   --> open animation  --> open  --> active  -->  |
 |                                                                    |  
 |  dispose <-- unload <-- close animation <-- close <-- deactive <-- |
 ----------------------------------------------------------------------

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.UI.Internal;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract partial class UIWindowBase
    {
        protected UIWindowBase()
        {
            _fetus = new WindowFetus(this);
        }

        // UI资源相关
        public abstract string GetResourcePath();
        public virtual RenderQueue GetRenderQueue() { return RenderQueue.Geometry; }
        
        // 事件相关: 以下6个事件, gameObject都是可用的, widget变量都是可用的
        
        /// <summary>
        /// 时机: 加载完成事件, 与OnUnloading()呼应. 此时window可见, widget可用, 但未执行open animation
        /// 用途: 控件数据初始化, register events
        /// </summary>
        public virtual void OnLoaded() {}
        
        /// <summary>
        /// 时机: 打开完成事件, 与OnClosing呼应. open animation执行完成, window等待玩家输入
        /// 用途: 
        /// </summary>
        public virtual void OnOpened() {}
        
        /// <summary>
        /// 时机: 获得焦点事件, 与OnDeactivating()呼应. 当通过OpenWindow()打开window时, 该window会被激活并收到OnActivated()事件
        /// 用途: 
        /// </summary>
        public virtual void OnActivated() {}
        
        /// <summary>
        /// 时机: 准备失去焦点事件, 与OnActivated()呼应. 当通用OpenWindow()打开其它window时, 当前位于前台的window会收到OnDeactivating()事件
        /// 用途: 
        /// </summary>
        public virtual void OnDeactivating() {}
        
        /// <summary>
        /// 时机: 准备关闭事件, 与OnOpened()呼应. 本事件执行完成后, 会开始close animation 
        /// 用途: 
        /// </summary>
        public virtual void OnClosing() {}
        
        /// <summary>
        /// 时机: 准备销毁事件, 与OnLoaded()呼应. gameObject与widgets尚未销毁, 都处于可用状态
        /// 用途: unregister在OnLoaded()中注册的events
        /// </summary>
        public virtual void OnUnloading() {}

        // 逻辑帧: 大概10fps
        // public virtual void LogicUpdate() {}
        // 正常Update
        public virtual void Update() {}

        internal virtual void Dispose()
        {
            if (_isReleased)
            {
                return;
            }

            _isReleased = true;
            UIManager._RemoveWindow(GetType());
            
            _fetus.Dispose();
            _fetus = null;
            _widgets.Clear();
        }

        internal WindowFetus GetFetus() { return _fetus; }
        public Transform GetTransform() { return _transform; }
        public Canvas GetCanvas() { return _canvas; }

        internal void _InitComponents(Transform transform, Canvas canvas)
        {
            _transform = transform;
            _canvas = canvas;
        }

        private WindowFetus _fetus;
        private Transform _transform;
        private Canvas _canvas;
        private  bool _isReleased;
    }
}