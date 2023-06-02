
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn
{
    internal class SafeEvent<T>
    {
        public void Call(T arg)
        {
            if (_handlers.Count == 0)
            {
                return;
            }

            lock (_handlerLocker)
            {
                var count = _handlers.Count;
                int i;
                for (i = 0; i < count; ++i)
                {
                    var weak = _handlers[i] as WeakObject;
                    var action = weak.Target as Action<T>;

                    if (null != action)
                    {
                        _HandleAction(action, arg);
                    }
                    else
                    {
                        break;
                    }
                }

                if (i == count)
                {
                    return;
                }

                int j;
                for (j = i + 1; j < count; ++j)
                {
                    var weak = _handlers[j] as WeakObject;
                    var action = weak.Target as Action<T>;
                    if (null != action)
                    {
                        _handlers[i] = _handlers[j];
                        ++i;

                        _HandleAction(action, arg);
                    }
                }

                var removedCount = j - i;
                if (removedCount > 0)
                {
                    _handlers.RemoveRange(i, removedCount);
                }
            }
        }

        private void _HandleAction(Action<T> action, T arg)
        {
            try
            {
                action(arg);
            }
            catch (Exception ex)
            {
                Logo.Error(ex);
            }
        }

        public void AddListener(Action<T> listener, out Action<T> lpfnListener)
        {
            // This is the strong reference
            lpfnListener = listener;

            if (null == listener)
            {
                return;
            }

            var weak = WeakObject.Spawn(listener);
            lock (_handlerLocker)
            {
                _handlers.Add(weak);
            }
        }

        public void RemoveListener(Action<T> listener)
        {
            if (null == listener)
            {
                return;
            }

            if (_handlers.Count == 0)
            {
                return;
            }

            lock (_handlerLocker)
            {
                var index = _Index(listener);
                if (index >= 0)
                {
                    _handlers.RemoveAt(index);
                }
            }
        }

        private int _Index(Action<T> listener)
        {
            var count = _handlers.Count;
            for (int i = 0; i < count; ++i)
            {
                var weak = _handlers[i] as WeakObject;
                var action = weak.Target as Action<T>;
                if (null != action && action.Target == listener.Target && action.Method == listener.Method)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RemoveAllListeners()
        {
            lock (_handlerLocker)
            {
                _handlers.Clear();
            }
        }

        private readonly ArrayList _handlers = new ArrayList();
        private readonly object _handlerLocker = new object();
    }
}