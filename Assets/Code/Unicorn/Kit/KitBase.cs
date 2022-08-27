
/********************************************************************
created:    2022-08-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine.Events;

namespace Unicorn
{
    public class KitBase
    {
        public virtual void Awake() { }
        public virtual void OnDestroy() { }
        public virtual void Update() { }

        public void AddListener(UnityEvent evt, UnityAction handler)
        {
            _listener.AddListener(evt, handler);
        }
        
        public void AddListener<T>(UnityEvent<T> evt, UnityAction<T> handler)
        {
            _listener.AddListener(evt, handler);
        }

        internal void RemoveAllListeners()
        {
            _listener.RemoveAllListeners();
        }
        
        public UnityEngine.Object[] GetAssets()
        {
            return _assets;
        }
        
        internal void _SetAssets(UnityEngine.Object[] assets)
        {
            _assets = assets;
        }
        
        private UnityEngine.Object[] _assets;
        private readonly EventListener _listener = new();
    }
}