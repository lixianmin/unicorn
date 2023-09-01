/********************************************************************
created:    2023-09-01
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
#if UNICORN_EDITOR

namespace Client.UI
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
            _name = name;
        }
        
        public string GetName()
        {
            return _name;
        }

        private int _tid;
        private string _name;
    }
}

#endif