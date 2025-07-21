/********************************************************************
created:    2023-09-01
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

#if UNICORN_EDITOR

using System.Collections.Generic;
using Unicorn.Collections;
using UnityEngine.Events;

namespace Clients.UI
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
            var goods = new ShopGood(tid, tid.ToString());
            _goods.Add(tid, goods);
            OnInsertGood.Invoke(goods);
        }

        public bool DeleteGoods(int tid)
        {
            var index = _goods.TryIndexValue(tid, out var goods);
            if (index >= 0)
            {
                _goods.RemoveAt(index);
                OnDeleteGood.Invoke(goods);
                return true;
            }

            return false;
        }

        public SortedTable<int, ShopGood>.ValueList GetGoods()
        {
            return _goods.Values;
        }

        public int GetNextId()
        {
            return ++_idGenerator;
        }

        public readonly UnityEvent OnInited = new();
        public readonly UnityEvent<ShopGood> OnInsertGood = new();
        public readonly UnityEvent<ShopGood> OnDeleteGood = new();

        private readonly SortedTable<int, ShopGood> _goods = new();

        public static readonly ShopManager It = new();
        private static int _idGenerator;
    }
}

#endif