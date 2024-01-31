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
            T item = default;
            var isOk = false;

            // lock (_locker)
            {
                if (_cache.Count > 0)
                {
                    item = _cache.PopBack() as T;
                    isOk = true;
                }
            }
 
            if (!isOk)
            {
                item = new T();
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

            _recycleAction?.Invoke(item);

            // lock (_locker)
            {
                _cache.PushBack(item);
            }
        }

        // private readonly object _locker = new();
        private readonly Deque _cache = new();

        private readonly Action<T> _spawnAction;
        private readonly Action<T> _recycleAction;

        private static readonly bool _isNullable = !typeof(T).IsSubclassOf(typeof(ValueType));
    }
}