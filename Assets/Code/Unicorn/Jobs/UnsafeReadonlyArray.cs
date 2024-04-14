/********************************************************************
created:    2024-03-16
author:     lixianmin

 https://github.com/ThousandAnt/ta-frustrum-culling.git

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unicorn
{
    public unsafe struct UnsafeReadonlyArray<T> : IDisposable where T : unmanaged
    {
        public UnsafeReadonlyArray(T[] data)
        {
            Length = data?.Length ?? throw new ArgumentNullException(nameof(data));

            _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            _ptr = (T*)_handle.AddrOfPinnedObject().ToPointer();
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }

        public T this[int i] => *(_ptr + i);
        public readonly int Length;

        [NativeDisableUnsafePtrRestriction] private readonly T* _ptr;
        private GCHandle _handle;
    }
}