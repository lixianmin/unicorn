
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
        protected virtual void Awake() { }
        
        /// <summary>
        /// 在 gameObject.SetActive(true) 或 script.enabled=true 时都会触发OnEnable()事件
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// 在 gameObject.SetActive(false) 或 script.enabled=false 时都会触发OnDisable()事件
        /// </summary>
        public virtual void OnDisable() { }

        public virtual void OnDestroy() { }
        
        public virtual void Update() { }
        
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
        /// 与MBKitProvider的同名属性同步
        /// </summary>
        public bool isActiveAndEnabled { get; internal set; }

        /// <summary>
        /// Kit对象的Update()顺序, 相同class的kit对象同一批调用Update().
        /// 如何通过sort控制Update()顺序呢? 可以在client代码中, 找一个地方使用以下方式初始化:
        ///   TypeTools.SetDefaultTypeIndex(typeof(Client.BowlKit));
        ///   TypeTools.SetDefaultTypeIndex(typeof(Client.PlayerMoveKit));
        /// </summary>
        internal int sort;

        private Transform _transform;
        private UnityEngine.Object[] _assets;
        private readonly EventListener _listener = new();
        private bool _isDisposed;
    }
}