
/********************************************************************
created:    2018-03-15
author:     lixianmin

Tick的顺序是按partId排序的，其实就是按Part创建的先后顺序进行的

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using Unicorn.Collections;

namespace Unicorn
{
    internal class PartTickSystem
    {
        static PartTickSystem()
        {

        }

        private PartTickSystem()
        {
            Entity.OnPartCreated += _OnPartCreated;
        }

        private void _OnPartCreated(IPart part)
        {
            ITickable tickable = part as ITickable;
            if (tickable != null && tickable is IIsDisposed)
            {
                _AddTickPart(tickable);
            }
        }

        internal void Tick()
        {
            if (_hasNewPart)
            {
                Array.Sort(_typeIndices, _tickParts);
                _hasNewPart = false;
            }

            var hasDisposed = _TickParts();

            if (hasDisposed)
            {
                _RemoveDisposedParts();
            }
        }

        private bool _TickParts()
        {
            var count = _size;
            if (count > 0)
            {
                var hasDisposed = false;
                for (int i = 0; i < count; ++i)
                {
                    var part = _tickParts[i];
                    var disposed = part as IIsDisposed;
                    if (null == disposed || !disposed.IsDisposed())
                    {
                        part.Tick();

                    }
                    else
                    {
                        hasDisposed = true;
                    }
                }

                return hasDisposed;
            }

            return false;
        }

        private void _RemoveDisposedParts()
        {
            int i;
            for (i = 0; i < _size; i++)
            {
                var disposed = _tickParts[i] as IIsDisposed;
                if (null != disposed && disposed.IsDisposed())
                {
                    break;
                }
            }

            if (i == _size)
            {
                return;
            }

            int j;
            for (j = i + 1; j < _size; j++)
            {
                var disposed = _tickParts[j] as IIsDisposed;
                if (null == disposed || !disposed.IsDisposed())
                {
                    _tickParts[i] = _tickParts[j];
                    _typeIndices[i] = _typeIndices[j];

                    ++i;
                }
            }

            var removedCount = j - i;
            if (removedCount > 0)
            {
                Array.Clear(_tickParts, i, removedCount);

                for (int k = 0; k < removedCount; ++k)
                {
                    _typeIndices[i + k] = _kMaxTypeIndex;
                }
            }

            _size = i;
        }

        private void _AddTickPart(ITickable part)
        {
            if (_size == _capacity)
            {
                _capacity <<= 1;
                var tickParts = new ITickable[_capacity];
                var typeIndices = new int[_capacity];

                Array.Copy(_tickParts, 0, tickParts, 0, _size);
                Array.Copy(_typeIndices, 0, typeIndices, 0, _size);

                for (int i = _size; i < _capacity; ++i)
                {
                    typeIndices[i] = _kMaxTypeIndex;
                }

                _tickParts = tickParts;
                _typeIndices = typeIndices;
            }

            _tickParts[_size] = part;
            _typeIndices[_size] = PartTypeIndices.SetDefaultTypeIndex(part.GetType());
            ++_size;

            _hasNewPart = true;
        }

        private const int _kMaxTypeIndex = 0x850506;
        private int[] _typeIndices = new int[] { _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex };
        private ITickable[] _tickParts = new ITickable[4];
        private int _capacity = 4;
        private int _size = 0;
        private bool _hasNewPart;

        public static readonly PartTickSystem Instance = new PartTickSystem();
    }
}