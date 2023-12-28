
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
        public static List<T> Spawn<T> ()
        {
            return InnerData<T>.Pool.Spawn();
        }
    
        public static void Recycle<T> (List<T> list)
        {
            InnerData<T>.Pool.Recycle(list);
        }
        
        private static class InnerData<T>
        {
            public static readonly ObjectPool<List<T>> Pool = new(null, list => list.Clear());
        }
    }
}
