
/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn.Collections;

namespace Unicorn
{
    public class ObjectPool<T> where T : class, new()
    {
        public ObjectPool(Action<T> spawnAction, Action<T> recycleAction)
        {
            _spawnAction = spawnAction;
            _recycleAction = recycleAction;
        }

        public T Get()
        {
            T item;

            if (_cache.Count > 0)
            {
                item = _cache.PopBack() as T;
            }
            else
            {
                item = new T();
                CountAll++;
            }

            _spawnAction?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            if (_isNullable && null == item)
            {
                return;
            }

            if (_cache.Count > 0 && ReferenceEquals(_cache.Back(), item))
            {
                Logo.Error("Internal error. Trying to destroy object that is already released to pool.");
                return;
            }

            _recycleAction?.Invoke(item);
            _cache.PushBack(item);
        }

        internal int CountAll { get; private set; }
        internal int CountActive => CountAll - CountInactive;
        internal int CountInactive => _cache.Count;

        private readonly Deque _cache = new();
        private readonly Action<T> _spawnAction;
        private readonly Action<T> _recycleAction;

        private static readonly bool _isNullable = !typeof(T).IsSubclassOf(typeof(ValueType));
    }
}