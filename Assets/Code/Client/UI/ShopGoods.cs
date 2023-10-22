/********************************************************************
created:    2023-09-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine.Events;

#if UNICORN_EDITOR

namespace Clients.UI
{
    public class ShopGoods
    {
        public ShopGoods(int tid, string name)
        {
            _tid = tid;
            _name = name;
        }

        public int GetTemplateId()
        {
            return _tid;
        }

        public void SetName(string name)
        {
            if (_name != name)
            {
                _name = name;
                OnUpdateGoods.Invoke(this);
            }
        }

        public string GetName()
        {
            return _name;
        }

        public readonly UnityEvent<ShopGoods> OnUpdateGoods = new();

        private int _tid;
        private string _name;
    }
}

#endif