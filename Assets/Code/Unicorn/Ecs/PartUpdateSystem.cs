
/********************************************************************
created:    2018-03-15
author:     lixianmin

Tick的顺序是按partId排序的，其实就是按Part创建的先后顺序进行的

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    internal class PartUpdateSystem
    {
        static PartUpdateSystem()
        {

        }

        internal PartUpdateSystem()
        {
            EntityBase.OnPartCreated += _OnPartCreated;
        }

        private void _OnPartCreated(IPart part)
        {
            var updatable = part as IExpensiveUpdater;
            if (updatable != null && updatable is IIsDisposed)
            {
                _AddUpdatePart(updatable);
            }
        }

        internal void ExpensiveUpdate(float deltaTime)
        {
            if (_hasNewPart)
            {
                Array.Sort(_typeIndices, _updateParts);
                _hasNewPart = false;
            }

            var hasDisposed = _UpdateParts(deltaTime);

            if (hasDisposed)
            {
                _RemoveDisposedParts();
            }
        }

        private bool _UpdateParts(float deltaTime)
        {
            var count = _size;
            if (count > 0)
            {
                var hasDisposed = false;
                for (var i = 0; i < count; ++i)
                {
                    var part = _updateParts[i];
                    var disposed = part as IIsDisposed;
                    if (null == disposed || !disposed.IsDisposed())
                    {
                        part.ExpensiveUpdate(deltaTime);
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
                var disposed = _updateParts[i] as IIsDisposed;
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
                var disposed = _updateParts[j] as IIsDisposed;
                if (null == disposed || !disposed.IsDisposed())
                {
                    _updateParts[i] = _updateParts[j];
                    _typeIndices[i] = _typeIndices[j];

                    ++i;
                }
            }

            var removedCount = j - i;
            if (removedCount > 0)
            {
                Array.Clear(_updateParts, i, removedCount);

                for (int k = 0; k < removedCount; ++k)
                {
                    _typeIndices[i + k] = _kMaxTypeIndex;
                }
            }

            _size = i;
        }

        private void _AddUpdatePart(IExpensiveUpdater part)
        {
            if (_size == _capacity)
            {
                _capacity <<= 1;
                var tickParts = new IExpensiveUpdater[_capacity];
                var typeIndices = new int[_capacity];

                Array.Copy(_updateParts, 0, tickParts, 0, _size);
                Array.Copy(_typeIndices, 0, typeIndices, 0, _size);

                for (int i = _size; i < _capacity; ++i)
                {
                    typeIndices[i] = _kMaxTypeIndex;
                }

                _updateParts = tickParts;
                _typeIndices = typeIndices;
            }

            _updateParts[_size] = part;
            _typeIndices[_size] = TypeTools.SetDefaultTypeIndex(part.GetType());
            ++_size;

            _hasNewPart = true;
        }

        private const int _kMaxTypeIndex = 0x850506;
        private int[] _typeIndices = { _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex };
        private IExpensiveUpdater[] _updateParts = new IExpensiveUpdater[4];
        private int _capacity = 4;
        private int _size = 0;
        private bool _hasNewPart;
    }
}