
/********************************************************************
created:    2022-09-05
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Collections;

namespace Unicorn
{
    internal class KitManager
    {
        static KitManager()
        {
            
        }

        private KitManager()
        {
            
        }

        public void Add(KitBase kit)
        {
            if (kit is not null)
            {
                _kits.Add(_idGenerator++, kit);
            }
        }
        
        private readonly SortedTable<int, KitBase> _kits = new();
        private int _idGenerator;
        
        public static readonly KitManager Instance = new();
    }
}