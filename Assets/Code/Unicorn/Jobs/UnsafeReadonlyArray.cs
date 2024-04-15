/********************************************************************
created:    2024-03-16
author:     lixianmin

 https://github.com/ThousandAnt/ta-frustrum-culling/blob/master/Assets/Scripts/ThousandAnt.FrustumCulling/Collections/UnsafeReadOnlyArray.cs

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

        /// <summary>
        /// 接收部分array, 为接收Slice<T>做准备
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UnsafeReadonlyArray(T[] data, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Length = length <= data.Length ? length : data.Length;
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