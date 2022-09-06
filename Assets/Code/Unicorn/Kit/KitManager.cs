
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

        public void Update()
        {
            if (_isDirty)
            {
                _kits._Sort();
                _isDirty = false;
            }
            
            
        }
        
        public void Add(KitBase kit)
        {
            if (kit is not null)
            {
                _kits._Append(_idGenerator++, kit);
                _isDirty = true;
            }
        }
        
        private readonly SortedTable<int, KitBase> _kits = new();
        private int _idGenerator;
        private bool _isDirty;
        
        public static readonly KitManager Instance = new();
    }
}