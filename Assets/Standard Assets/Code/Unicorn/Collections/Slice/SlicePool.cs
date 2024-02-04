/********************************************************************
created:    2023-12-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Collections
{
    public static class SlicePool
    {
        public static Slice<T> Get<T> ()
        {
            return InnerData<T>.Pool.Get();
        }
    
        public static void Return<T> (Slice<T> slice)
        {
            InnerData<T>.Pool.Return(slice);
        }
        
        private static class InnerData<T>
        {
            public static readonly ObjectPool<Slice<T>> Pool = new(null, slice => slice.Clear());
        }
    }
}