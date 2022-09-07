
/********************************************************************
created:    2022-09-06
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

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

        public static void Search(string kitName, List<string> results)
        {
            kitName ??= string.Empty;
            foreach (var key in _lookupTable.Keys)
            {
                var fullKitName = key as string;
                if (fullKitName!.IndexOf(kitName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    results.Add(fullKitName);
                }
            }
                
            results.Sort();
        }
        
        private static readonly Hashtable _lookupTable;
    }
}