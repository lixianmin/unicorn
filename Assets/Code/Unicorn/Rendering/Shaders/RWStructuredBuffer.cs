/********************************************************************
created:    2024-02-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unicorn
{
    public class RWStructuredBuffer<T> : StructuredBuffer
    {
        public RWStructuredBuffer(string name, int stride) : base(name, stride)
        {
            _lpfnOnAsyncGPUReadback = _OnAsyncGPUReadback;
        }

        public T[] GetData()
        {
            if (!IsDisposed())
            {
                var buffer = GetBuffer();
                _ResizeData(buffer);

                buffer.GetData(_data);
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
                _ResizeData(buffer);

                AsyncGPUReadback.Request(buffer, _lpfnOnAsyncGPUReadback);
                return _data;    
            }

            return EmptyArray<T>.It;
        }

        private void _ResizeData(ComputeBuffer buffer)
        {
            var count = buffer.count;
            if (_data?.Length != count)
            {
                _data = new T[count];
            }
        }

        private void _OnAsyncGPUReadback(AsyncGPUReadbackRequest request)
        {
            var buffer = GetBuffer();
            if (!request.hasError && buffer.IsValid())
            {
                buffer.GetData(_data);
            }
        }

        private readonly Action<AsyncGPUReadbackRequest> _lpfnOnAsyncGPUReadback;
        private T[] _data;
    }
}