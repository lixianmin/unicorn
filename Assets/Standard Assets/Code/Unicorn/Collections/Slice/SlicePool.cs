/********************************************************************
created:    2023-12-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn.Collections
{
    public static class SlicePool
    {
        public class Slice2<T> : Slice<T>, IDisposable
        {
            void IDisposable.Dispose()
            {
                Return(this);
            }

            internal bool IsDisposed;
        }

        public static Slice2<T> Rent<T>(params T[] inputs)
        {
            var slice = InnerData<T>.Pool.Rent();
            foreach (var item in inputs)
            {
                slice.Add(item);
            }

            return slice;
        }

        /// <summary>
        /// 并不是所有的slice都可以使用 using 自动ReturnPool, 有时候我们会定义为类成员变量, 就需要主动return一把了
        /// </summary>
        /// <param name="slice"></param>
        /// <typeparam name="T"></typeparam>
        public static void Return<T>(Slice<T> slice)
        {
            if (slice is Slice2<T> { IsDisposed: false } it)
            {
                it.IsDisposed = true;
                InnerData<T>.Pool.Return(it);
            }
        }

        private static class InnerData<T>
        {
            public static readonly ObjectPool<Slice2<T>> Pool = new(slice => slice.IsDisposed = false,
                slice => slice.Clear());
        }
    }
}