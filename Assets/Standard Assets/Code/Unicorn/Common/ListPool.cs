/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static class ListPool
    {
        public class List2<T> : List<T>, IDisposable
        {
            void IDisposable.Dispose()
            {
                Return(this);
            }

            internal bool IsDisposed;
        }

        public static List2<T> Rent<T>(params T[] inputs)
        {
            var list = InnerData<T>.Pool.Rent();
            foreach (var item in inputs)
            {
                list.Add(item);
            }

            return list;
        }

        public static void Return<T>(List<T> list)
        {
            if (list is List2<T> { IsDisposed: false } it)
            {
                it.IsDisposed = true;
                InnerData<T>.Pool.Return(it);
            }
        }

        private static class InnerData<T>
        {
            public static readonly ObjectPool<List2<T>> Pool = new(null, list => list.Clear());
        }
    }
}