
/********************************************************************
created:    2022-08-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEngine.Events;

namespace Unicorn
{
    public class KitBase: IIsDisposed
    {
        public virtual void Awake() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }

        /// <summary>
        /// AddListener()这个方法是送的, 子类可以用, 也可以不用
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="handler"></param>
        public void AddListener(UnityEvent evt, UnityAction handler)
        {
            _listener.AddListener(evt, handler);
        }
        
        public void AddListener<T>(UnityEvent<T> evt, UnityAction<T> handler)
        {
            _listener.AddListener(evt, handler);
        }

        bool IIsDisposed.IsDisposed()
        {
            return _isDisposed;
        }

        public Transform GetTransform()
        {
            return _transform;
        }
        
        public UnityEngine.Object[] GetAssets()
        {
            return _assets;
        }
        
        internal void _Init(Transform transform, UnityEngine.Object[] assets)
        {
            _transform = transform;
            _assets = assets;
            
            KitManager.Instance.Add(this);
            CallbackTools.Handle(Awake, "[Awake()]");
        }

        internal void _Dispose()
        {
            CallbackTools.Handle(OnDestroy, "[OnDestroy()]");
            _listener.RemoveAllListeners();
            _isDisposed = true;
        }

        /// <summary>
        /// Kit对象的Update()顺序, 默认相同class的kit对象同一批更新
        /// </summary>
        internal int sort;
        
        private Transform _transform;
        private UnityEngine.Object[] _assets;
        private readonly EventListener _listener = new();
        private bool _isDisposed;
    }
}