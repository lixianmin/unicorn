/********************************************************************
created:    2023-12-28
author:     lixianmin

List<T>的替代品, 之所以出现这个类, 是因为很多方法只能传递数组T[], 而List<T>提供不了这样的功能

但是, 代价是需要更加小心的控制代码逻辑, 以防止其它代码拿到裸数据后意外修改.

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Collections
{
    public partial class Slice<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        public T[] Items;
        public int Size;

        public Slice() : this(4)
        {
        }

        public Slice(int capacity)
        {
            const int minSize = 1;
            capacity = Math.Max(capacity, minSize);
            Items = new T[capacity];
        }

        public void Add(T item)
        {
            ArrayTools.AddItem(ref Items, ref Size, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException("index is out of range.");
            }

            --Size;

            if (index < Size)
            {
                Array.Copy(Items, index + 1, Items, index, Size - index);
            }

            Items[Size] = default;
        }

        public void AddRange(Slice<T> others)
        {
            if (others is { Size: > 0 })
            {
                var nextCount = Size + others.Size;
                Reserve(nextCount);

                Array.Copy(others.Items, 0, Items!, Size, others.Size);
                Size = nextCount;
            }
        }

        public void Reserve(int minCapacity)
        {
            var capacity = Items?.Length ?? 0;
            if (minCapacity > capacity)
            {
                var nextCapacity = ArrayTools.EnsureCapacity(capacity, minCapacity);
                Array.Resize(ref Items, nextCapacity);
            }
        }

        public void Clear()
        {
            if (Size > 0)
            {
                Array.Clear(Items, 0, Size);
                Size = 0;
            }
        }

        public int IndexOf(T item)
        {
            for (var index = 0; index < Size; ++index)
            {
                if (Equals(Items[index], item))
                {
                    return index;
                }
            }

            return -1;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(Items, 0, array, arrayIndex, Size);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
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

        public int Count => Size;
        public bool IsReadOnly => false;
    }
}