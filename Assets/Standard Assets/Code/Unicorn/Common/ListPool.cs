
/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;

namespace Unicorn
{
    public static class ListPool
    {
        public static List<T> Get<T> ()
        {
            return InnerData<T>._pool.Get();
        }
    
        public static void Return<T> (List<T> list)
        {
            InnerData<T>._pool.Return(list);
        }
        
        private static class InnerData<T>
        {
            public static readonly ObjectPool<List<T>> _pool = new(null, list => list.Clear());
        }
    }
}
