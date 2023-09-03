/********************************************************************
created:    2023-09-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

#if UNICORN_EDITOR

using System.Collections.Generic;
using Unicorn.Collections;
using UnityEngine.Events;

namespace Client.UI
{
    public class ShopManager
    {
        private ShopManager()
        {
            for (var i = 0; i < 14; i++)
            {
                var tid = GetNextId();
                InsertGoods(tid);
            }
        }
        
        public void InsertGoods(int tid)
        {
            var goods = new ShopGoods(tid, tid.ToString());
            _goods.Add(tid, goods);
            OnInsertGoods.Invoke(goods);
        }

        public bool DeleteGoods(int tid)
        {
            var index = _goods.TryIndexValue(tid, out var goods);
            if (index >= 0)
            {
                _goods.RemoveAt(index);
                OnDeleteGoods.Invoke(goods);
                return true;
            }

            return false;
        }

        public IEnumerable<ShopGoods> GetEnumerator()
        {
            foreach (var goods in _goods.Values)
            {
                yield return goods;
            }
        }

        public int GetNextId()
        {
            return ++_idGenerator;
        }

        public readonly UnityEvent OnInited = new();
        public readonly UnityEvent<ShopGoods> OnInsertGoods = new();
        public readonly UnityEvent<ShopGoods> OnDeleteGoods = new();

        private readonly SortedTable<int, ShopGoods> _goods = new();

        public static readonly ShopManager It = new();
        private static int _idGenerator;
    }
}

#endif