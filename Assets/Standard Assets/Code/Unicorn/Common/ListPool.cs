
/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections.Generic;

namespace Unicorn
{
    public static class ListPool<T>
    {
        public static List<T> Spawn ()
        {
            return _pool.Spawn();
        }

        public static void Recycle (List<T> list)
        {
            _pool.Recycle(list);
        }

        private static readonly ObjectPool<List<T>> _pool = new ObjectPool<List<T>>(null, list => list.Clear());
    }

    public static class ListPool
    {
        public static List<object> Spawn ()
        {
            return _pool.Spawn();
        }

        public static void Recycle (List<object> list)
        {
            _pool.Recycle(list);
        }

        private static readonly ObjectPool<List<object>> _pool = new ObjectPool<List<object>>(null, list => list.Clear());
    }
}
