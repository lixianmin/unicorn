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

            public bool IsDisposed;
        }

        public static Slice2<T> Get<T>()
        {
            return InnerData<T>.Pool.Get();
        }

        /// <summary>
        /// 并不是所有的slice都可以使用 using 自动ReturnPool, 有时候我们会定义为类成员变量, 就需要主动return一把了
        /// </summary>
        /// <param name="slice"></param>
        /// <typeparam name="T"></typeparam>
        public static void Return<T>(Slice2<T> slice)
        {
            if (slice is { IsDisposed: false })
            {
                slice.IsDisposed = true;
                InnerData<T>.Pool.Return(slice);
            }
        }

        private static class InnerData<T>
        {
            public static readonly ObjectPool<Slice2<T>> Pool = new(slice=> slice.IsDisposed = false, slice => slice.Clear());
        }
    }
}