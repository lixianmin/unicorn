
/********************************************************************
created:    2024-10-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public class Entity : EntityBase, IDisposable, IIsDisposed
    {
        ~Entity()
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
                _DoDispose(flags);
                Clear();
            }
            finally
            {
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        protected virtual void _DoDispose(int flags)
        {

        }

        private bool _isDisposed;
    }
}