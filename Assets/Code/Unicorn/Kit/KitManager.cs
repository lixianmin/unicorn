
/********************************************************************
created:    2022-09-05
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
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
                // _kits._Sort();
                _isDirty = false;
            }
            
            
        }
        
        public void Add(KitBase kit)
        {
            if (kit is not null)
            {
                // _kits._Append(_idGenerator++, kit);
                _isDirty = true;
            }
        }
        
        private KitBase[] _kits = Array.Empty<KitBase>();
        private int[] _priorities= Array.Empty<int>();
        
        private int _idGenerator;
        private bool _isDirty;
        
        public static readonly KitManager Instance = new();
    }
}