/********************************************************************
created:    2014-06-25
author:     lixianmin

1. 确保_DoDispose()方法是从游戏主线程调用过来的, 而不是从GC线程
2. 确保_DoDispose()会且只会调用一次: 如果不手动调用, 则会被GC辗转调用到

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public abstract class Disposable : IDisposable, IIsDisposed
    {
        ~Disposable()
        {
            DisposableRecycler.Recycle(this);
        }

        public void Dispose()
        {
            Dispose(0);
        }

        public void Dispose(int flags)
        {
            if (IsDisposed())
            {
                return;
            }
            
            try
            {
                // 在_DoDispose()之前设置 _isDisposed = true, 以防止递归调用自己
                _isDisposed = true;
                _DoDispose(flags);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        protected abstract void _DoDispose(int flags);

        private bool _isDisposed;
    }
}