/********************************************************************
created:    2024-02-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn.Rendering
{
    public class ComputeKernel
    {
        public ComputeKernel(ComputeShader shader, string kernelName)
        {
            if (shader == null)
            {
                throw new ArgumentNullException(nameof(shader));
            }

            if (kernelName.IsNullOrEmpty())
            {
                throw new ArgumentException("empty kernelName");
            }

            _shader = shader;
            _kernelIndex = shader.FindKernel(kernelName);
            shader.GetKernelThreadGroupSizes(_kernelIndex, out _threadGroupSizeX, out _, out _);
        }

        public void SetBuffer(string name, ComputeBuffer buffer)
        {
            _shader.SetBuffer(_kernelIndex, name, buffer);
        }

        // public void SetBuffer(StructuredBuffer buffer)
        // {
        //     if (buffer != null)
        //     {
        //         _shader.SetBuffer(_kernelIndex, buffer.GetName(), buffer.GetBuffer());
        //     }
        // }

        public void SetBuffer(StructuredBuffer buffer, Array data)
        {
            if (buffer != null && data != null)
            {
                buffer.SetData(data);
                _shader.SetBuffer(_kernelIndex, buffer.GetNameId(), buffer.GetBuffer());
            }
        }

        public void SetBuffer<T>(RWStructuredBuffer<T> buffer, int size)
        {
            if (buffer != null)
            {
                buffer.Reserve(size); // 这个用于取回数据, 所以只需要设置size
                _shader.SetBuffer(_kernelIndex, buffer.GetNameId(), buffer.GetBuffer());
            }
        }
        
        public void SetBuffer<T>(AppendStructuredBuffer<T> buffer, int size)
        {
            if (buffer != null)
            {
                buffer.Reserve(size); // 这个用于取回数据, 所以只需要设置size

                var backBuffer = buffer.GetBuffer();
                backBuffer.SetCounterValue(0);
                _shader.SetBuffer(_kernelIndex, buffer.GetNameId(), backBuffer);
            }
        }

        public void Dispatch(int size)
        {
            var threadGroupsX = size / (int)_threadGroupSizeX + 1;
            _shader.Dispatch(_kernelIndex, threadGroupsX, 1, 1);
        }

        private readonly ComputeShader _shader;
        private readonly int _kernelIndex;
        private readonly uint _threadGroupSizeX;
    }
}