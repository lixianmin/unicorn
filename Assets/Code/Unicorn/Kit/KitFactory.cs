
/********************************************************************
created:    2022-09-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn
{
    public static class KitFactory
    {
        static KitFactory()
        {
            var kitFactoryType = TypeTools.SearchType("Unicorn._KitFactory");
            if (null != kitFactoryType)
            {
                TypeTools.CreateDelegate(kitFactoryType, "_GetLookupTableByName", out Func<Hashtable> method);
                if (method != null)
                {
                    _lookupTable = method();
                }
            }

            _lookupTable ??= new Hashtable();
        }
        
        public static KitBase Create(string fullKitName)
        {
            var key = (fullKitName ?? string.Empty).Trim();
            if (_lookupTable[key] is Func<KitBase> creator && creator() is { } kit)
            {
                return kit;
            }

            return null;
        }
        
        private static readonly Hashtable _lookupTable;
    }
}