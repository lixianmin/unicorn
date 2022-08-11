
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

        public T Spawn()
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

            if (null != _spawnAction)
            {
                _spawnAction(item);
            }

            return item;
        }

        public void Recycle(T item)
        {
            if (_isNullable && null == item)
            {
                return;
            }

            if (_cache.Count > 0 && ReferenceEquals(_cache.Back(), item))
            {
                Console.Error.WriteLine("Internal error. Trying to destroy object that is already released to pool.");
                return;
            }

            if (null != _recycleAction)
            {
                _recycleAction(item);
            }

            _cache.PushBack(item);
        }

        internal int CountAll { get; private set; }
        internal int CountActive { get { return CountAll - CountInactive; } }
        internal int CountInactive { get { return _cache.Count; } }

        private readonly Deque _cache = new Deque();
        private readonly Action<T> _spawnAction;
        private readonly Action<T> _recycleAction;

        private static readonly bool _isNullable = !typeof(T).IsSubclassOf(typeof(ValueType));
    }
}