/********************************************************************
created:    2024-02-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    public class ComputeKernel
    {
        private struct ThreadGroup
        {
            public int Num;

            private int _groupSize;
            private int _lastWidth;

            public void SetGroupSize(uint size)
            {
                _groupSize = (int)size;
            }

            public void Update(int width)
            {
                if (_lastWidth != width)
                {
                    _lastWidth = width;
                    Num = width / _groupSize;
                    if (Num * _groupSize < width)
                    {
                        Num++;
                    }
                }
            }
        }

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

            shader.GetKernelThreadGroupSizes(_kernelIndex, out var x, out var y, out _);
            _threadGroupX.SetGroupSize(x);
            _threadGroupY.SetGroupSize(y);
        }


        public void SetTexture(string name, Texture texture)
        {
            _shader.SetTexture(_kernelIndex, name, texture);
        }

        public void SetBuffer(string name, ComputeBuffer buffer)
        {
            _shader.SetBuffer(_kernelIndex, name, buffer);
        }

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

        public void Dispatch(int width, int height = 1)
        {
            _threadGroupX.Update(width);
            _threadGroupY.Update(height);
            _shader.Dispatch(_kernelIndex, _threadGroupX.Num, _threadGroupY.Num, 1);
        }

        private readonly ComputeShader _shader;
        private readonly int _kernelIndex;

        private ThreadGroup _threadGroupX;
        private ThreadGroup _threadGroupY;
    }
}