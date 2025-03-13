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
                fetus.SetActive(true);
                _cache.Remove(type);
            }

            // Logo.Warn($"[TakeFetus] type={type} fetus={fetus} _cache={_cache.Count}");
            return fetus;
        }

        public void AddFetus(WindowFetus fetus)
        {
            var transform = fetus?.GetTransform();
            if (transform == null)
            {
                return;
            }

            transform.gameObject.SetActive(false);
            var type = fetus.master.GetType();
            _cache[type] = fetus;

            // Logo.Warn($"[AddFetus] type={type} fetus={fetus} _cache={_cache.Count}");
        }

        public static readonly FetusCache It = new();

        private readonly Hashtable _cache = new();
    }
}