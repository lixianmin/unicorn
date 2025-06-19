/********************************************************************
created:    2024-10-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public partial class EntityBase
    {
        public void Dispose()
        {
            if (IsDisposed())
            {
                return;
            }

            try
            {
                // 在_DoDispose()之前设置 _isDisposed = true, 以防止递归调用自己
                _isDisposed = true;

                if (OnDisposing != null)
                {
                    OnDisposing();
                    OnDisposing = null;
                }

                Clear();
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

        public event Action OnDisposing;
        private bool _isDisposed;
    }
}