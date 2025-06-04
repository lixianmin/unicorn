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
                InnerData<T>.Pool.Return(this);
            }
        }

        public static Slice2<T> Get<T>()
        {
            return InnerData<T>.Pool.Get();
        }

        // public static void Return<T>(Slice2<T> slice)
        // {
        //     if (slice is { IsDisposed: false })
        //     {
        //         slice.IsDisposed = true;
        //         InnerData<T>.Pool.Return(slice);
        //     }
        // }

        private static class InnerData<T>
        {
            public static readonly ObjectPool<Slice2<T>> Pool = new(null, slice => slice.Clear());
        }
    }
}