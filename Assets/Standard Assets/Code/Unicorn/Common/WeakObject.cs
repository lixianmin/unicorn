
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Runtime.InteropServices;
using Unicorn.Collections;

namespace Unicorn
{
    internal class WeakObject : IDisposable
    {
        public static WeakObject Spawn(object target)
        {
            WeakObject weak = null;
            if (_weakObjectCache.Count > 0)
            {
                lock (_cacheLock)
                {
                    if (_weakObjectCache.Count > 0)
                    {
                        weak = _weakObjectCache.Back() as WeakObject;
                        _weakObjectCache.PopBack();
                    }
                }
            }

            weak = weak ?? new WeakObject();
            weak.Target = target;
            return weak;
        }

        public static void Recycle(WeakObject weak)
        {
            var disposable = weak as IDisposable;
            if (null != disposable)
            {
                disposable.Dispose();
            }
        }

        private WeakObject()
        {
            _gcHandle = GCHandle.Alloc(null, GCHandleType.Weak);
        }

        ~WeakObject()
        {
            _DoDispose(false);
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);
            _DoDispose(true);
        }

        private void _DoDispose(bool isManualDisposing)
        {
            _isDisposed = true;
            Target = null;

            lock (_cacheLock)
            {
                _weakObjectCache.PushBack(this);
            }
        }

        public object Target
        {
            get
            {
                if (!_gcHandle.IsAllocated)
                {
                    return null;
                }

                return _gcHandle.Target;
            }

            set
            {
                _gcHandle.Target = value;
            }
        }

        private GCHandle _gcHandle;
        private bool _isDisposed;

        private static object _cacheLock = new object();
        private static Deque _weakObjectCache = new Deque();
    }
}