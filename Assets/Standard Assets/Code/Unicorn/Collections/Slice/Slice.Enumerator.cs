﻿/********************************************************************
created:    2023-12-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Collections
{
    partial class Slice<T>
    {
        public struct Enumerator
        {
            public Enumerator(Slice<T> slice)
            {
                _slice = slice;
                _current = -1;
            }

            public bool MoveNext()
            {
                if (_current >= _slice.Size - 1)
                {
                    _current = -1;
                    return false;
                }

                _current++;
                return true;
            }

            public void Reset()
            {
                _current = -1;
            }

            public T Current
            {
                get
                {
                    if (_current < 0 || _current >= _slice.Size)
                    {
                        throw new InvalidOperationException();
                    }

                    return _slice.Items[_current];
                }
            }

            // caution: 这里故意没有记录version, 就需要程序自己来保证在foreach的过程中不要修改slice
            private readonly Slice<T> _slice;
            private int _current;
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _GetEnumerator();
        }

        private IEnumerator<T> _GetEnumerator()
        {
            for (var i = 0; i < Size; ++i)
            {
                var result = Items[i];
                yield return result;
            }
        }
    }
}