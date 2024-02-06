/********************************************************************
created:    2024-02-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    public class StructuredBuffer : Disposable
    {
        public StructuredBuffer(string name, int stride, ComputeBufferType type = ComputeBufferType.Default)
        {
            const int defaultSize = 8;
            _buffer = new ComputeBuffer(defaultSize, stride, type);
            _type = type;
            _nameId = Shader.PropertyToID(name);
            _stride = stride;
        }

        public void SetData(Array data)
        {
            if (data != null)
            {
                Reserve(data.Length);
                _buffer.SetData(data);
            }
        }

        public void Reserve(int size)
        {
            if (_buffer.count < size)
            {
                _buffer.Release();
                _buffer = new ComputeBuffer(size, _stride, _type);
            }
        }

        protected override void _DoDispose(int flags)
        {
            _buffer.Release();
        }

        public ComputeBuffer GetBuffer()
        {
            return _buffer;
        }

        public int GetNameId()
        {
            return _nameId;
        }

        public bool IsValid()
        {
            return _buffer.IsValid();
        }

        public virtual int Count => _buffer.count;

        private ComputeBuffer _buffer;
        private readonly int _nameId;
        private readonly int _stride;
        private readonly ComputeBufferType _type;
    }
}