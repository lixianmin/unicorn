/********************************************************************
created:    2025-03-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using Unicorn.UI.States;

namespace Unicorn.UI.Internal
{
    internal class WindowCache
    {
        public UIWindowBase TakeWindow(Type windowType)
        {
            if (_cache[windowType] is UIWindowBase window)
            {
                _cache.Remove(windowType);

                var transform = window.GetTransform();
                if (transform != null)
                {
                    window.ResetDispose();
                    window.GetFetus().ChangeState(StateKind.Load);
                    transform.gameObject.SetActive(true);
                    return window;
                }
            }

            // Logo.Warn($"[TakeFetus] type={type} fetus={fetus} _cache={_cache.Count}");
            return null;
        }

        public void AddWindow(UIWindowBase window)
        {
            var transform = window?.GetTransform();
            if (transform == null)
            {
                return;
            }

            transform.gameObject.SetActive(false);
            var type = window.GetType();
            _cache[type] = window;

            // Logo.Warn($"[AddFetus] type={type} fetus={fetus} _cache={_cache.Count}");
        }

        public static readonly WindowCache It = new();

        private readonly Hashtable _cache = new();
    }
}