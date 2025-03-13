/********************************************************************
created:    2025-03-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn.UI.Internal
{
    internal class FetusCache
    {
        public WindowFetus TakeFetus(Type type)
        {
            var fetus = _cache[type] as WindowFetus;
            if (fetus != null)
            {
                _cache.Remove(type);
            }
            
            return fetus;
        }

        public void AddFetus(WindowFetus fetus)
        {
            _cache[fetus.master.GetType()] = fetus;
        }
        
        private readonly Hashtable _cache = new();
    }
}