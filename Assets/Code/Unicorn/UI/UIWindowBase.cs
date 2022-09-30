
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
        
        // 加载事件相关
        public virtual void OnLoaded() {}
        public virtual void OnOpened() {}
        public virtual void OnActivated() {}
        public virtual void OnDeactivating() {}
        public virtual void OnClosing() {}
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