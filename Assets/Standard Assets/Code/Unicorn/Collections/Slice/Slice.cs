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
    public partial class Slice<T> : IEnumerable<T>
    {
        public T[] Items;
        public int Count;

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
            ArrayTools.AddItem(ref Items, ref Count, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range.");
            }

            --Count;

            if (index < Count)
            {
                Array.Copy(Items, index + 1, Items, index, Count - index);
            }

            Items[Count] = default;
        }

        public void AddRange(Slice<T> others)
        {
            if (others is { Count: > 0 })
            {
                var nextCount = Count + others.Count;
                Reserve(nextCount);

                Array.Copy(others.Items, 0, Items!, Count, others.Count);
                Count = nextCount;
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
            if (Count > 0)
            {
                Array.Clear(Items, 0, Count);
                Count = 0;
            }
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
            for (var i = 0; i < Count; ++i)
            {
                var result = Items[i];
                yield return result;
            }
        }
    }
}