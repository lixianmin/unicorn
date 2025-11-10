/********************************************************************
created:    2025-03-12
author:     lixianmin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
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
                    window.ResetDisposed();
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