/********************************************************************
created:    2024-02-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unicorn
{
    public class AppendStructuredBuffer<T> : StructuredBuffer
    {
        public AppendStructuredBuffer(string name, int stride) : base(name, stride, ComputeBufferType.Append)
        {
            _lpfnOnAsyncGPUReadback = _OnAsyncGPUReadback;
            _argBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);

            AtDisposing += () =>
            {
                _argBuffer.Dispose();
                _argsCount[0] = 0; // 重置count=0
            };
        }

        public T[] GetData()
        {
            if (!IsDisposed())
            {
                var backBuffer = GetBuffer();
                ComputeBuffer.CopyCount(backBuffer, _argBuffer, 0);
                _argBuffer.GetData(_argsCount);

                var count = _argsCount[0];
                if (_data?.Length < count)
                {
                    _data = new T[count];
                }

                backBuffer.GetData(_data);
                return _data;
            }

            return EmptyArray<T>.It;
        }

        public T[] GetDataAsync()
        {
            // 取回这一步导致大量的等待同步时间, 太慢了, 还不如单独在主线程上执行
            // 最好的是使用流水线的方案, 就是CPU和GPU都不相互等待, 不同的CPU之间也不相互等待, 各做各的事
            // 
            // 如果想使用Graphics.DrawMeshInstancedIndirect()不取回数据, 直接在GPU上绘制的话, 则需要写一个
            // 支持ComputeBuffer的fragment shader配合才行
            // 
            // https://dev.to/alpenglow/unity-fast-pixel-reading-part-2-asyncgpureadback-4kgn
            // 通过 AsyncGPUReadback.Request(buffer) 可以异步把数据从GPU取回, 但这样这个buffer就无法复用了

            if (!IsDisposed())
            {
                var buffer = GetBuffer();
                AsyncGPUReadback.Request(buffer, _lpfnOnAsyncGPUReadback);
                return _data;
            }

            return EmptyArray<T>.It;
        }

        private void _OnAsyncGPUReadback(AsyncGPUReadbackRequest request)
        {
            var backBuffer = GetBuffer();
            if (!request.hasError && backBuffer.IsValid())
            {
                GetData();
            }
        }

        public override int Count => _argsCount[0];

        private readonly Action<AsyncGPUReadbackRequest> _lpfnOnAsyncGPUReadback;
        private readonly ComputeBuffer _argBuffer;
        private readonly int[] _argsCount = { 0 };
        private T[] _data = EmptyArray<T>.It;
    }
}