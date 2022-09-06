
/********************************************************************
created:    2022-09-05
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn.Collections;

namespace Unicorn
{
    internal class KitManager
    {
        static KitManager()
        {
        }

        private KitManager()
        {
        }

        public void Update()
        {
            if (_dirty)
            {
                Array.Sort(_sorts, _kits);
                _dirty = false;
            }

            var hasDisposed = _UpdateKits();
            if (hasDisposed)
            {
                _RemoveDisposedKits();
            }
        }
        
        private bool _UpdateKits()
        {
            var count = _size; // 在update的过程中, 即使_size大小改变了也无影响
            var hasDisposed = false;
            for (var i = 0; i < count; ++i)
            {
                var kit = _kits[i];
                var disposed = (IIsDisposed)kit;
                if (!disposed.IsDisposed())
                {
                    kit.Update();
                }
                else
                {
                    hasDisposed = true;
                }
            }

            return hasDisposed;
        }
        
        private void _RemoveDisposedKits()
        {
            int i;
            for (i = 0; i < _size; i++)
            {
                var kit = _kits[i];
                var disposed = (IIsDisposed) kit ;
                if (disposed.IsDisposed())
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
                var kit = _kits[i];
                var disposed = (IIsDisposed) kit ;
                if (!disposed.IsDisposed())
                {
                    _sorts[i] = _sorts[j];
                    _kits[i] = _kits[j];

                    ++i;
                }
            }

            var removedCount = j - i;
            if (removedCount > 0)
            {
                Array.Clear(_kits, i, removedCount);
                Array.Clear(_sorts, i, removedCount);
            }

            _size = i;
        }
        
        public void Add(KitBase kit)
        {
            if (kit is not null)
            {
                if (_size == _capacity)
                {
                    _capacity <<= 1;
                    
                    var sorts = new int[_capacity];
                    var kits = new KitBase[_capacity];
                    Array.Copy(_sorts, 0, sorts, 0, _size);
                    Array.Copy(_kits, 0, kits, 0, _size);
                    
                    _sorts = sorts;
                    _kits = kits;
                }
                
                _sorts[_size] = TypeTools.SetDefaultTypeIndex(kit.GetType());
                _kits[_size] = kit;

                ++_size;
                _dirty = true;
            }
        }

        private const int _initialSize = 4;
        
        private int _size;
        private int _capacity = _initialSize;
        private bool _dirty;

        private int[] _sorts = new int[_initialSize];
        private KitBase[] _kits = new KitBase[_initialSize];

        public static readonly KitManager Instance = new ();
    }
}