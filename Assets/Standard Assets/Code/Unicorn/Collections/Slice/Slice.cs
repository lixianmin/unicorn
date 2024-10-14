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
    public partial class Slice<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable
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

        public void Insert(int index, T item)
        {
            _CheckIndex(index);

            var nextSize = Size + 1;
            var capacity = Items.Length;
            if (nextSize > capacity)
            {
                var nextCapacity = ArrayTools.EnsureCapacity(capacity, nextSize);
                Array.Resize(ref Items, nextCapacity);
            }

            if (index < Size)
            {
                Array.Copy(Items, index, (Array)Items, index + 1, Size - index);
            }

            Items[index] = item;
            ++Size;
        }

        public void RemoveAt(int index)
        {
            _CheckIndex(index);
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
                var nextSize = Size + others.Size;
                Reserve(nextSize);

                Array.Copy(others.Items, 0, Items!, Size, others.Size);
                Size = nextSize;
            }
        }

        public void AddRange(List<T> others)
        {
            var othersCount = others?.Count ?? 0;
            if (othersCount > 0)
            {
                var nextSize = Size + othersCount;
                Reserve(nextSize);

                others.CopyTo(Items, Size);
                Size = nextSize;
            }
        }

        // public void AddRange(IList<T> others)
        // {
        //     if (others != null)
        //     {
        //         var count = others.Count;
        //         for (int i = 0; i < count; i++)
        //         {
        //             var item = others[i];
        //             ArrayTools.AddItem(ref Items, ref Size, item);
        //         }
        //     }
        // }



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

        public T this[int index]
        {
            get
            {
                _CheckIndex(index);
                return Items[index];
            }

            set
            {
                _CheckIndex(index);
                Items[index] = value;
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
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

        private void _CheckIndex(int index)
        {
            if (index < 0 || index >= Size)
            {
                throw new ArgumentOutOfRangeException("index is out of range.");
            }
        }

        public int Count => Size;
    }
}