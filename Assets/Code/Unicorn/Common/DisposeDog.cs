/********************************************************************
created:    2024-02-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
    public class DisposeDog : Disposable
    {
        public T Add<T>(T item) where T : IDisposable
        {
            if (item != null)
            {
                _items.Add(item);
            }

            return item;
        }

        public void DisposeAnClear()
        {
            if (_items.Count > 0)
            {
                foreach (var item in _items)
                {
                    item.Dispose();
                }

                _items.Clear();
            }
        }

        protected override void _DoDispose(int flags)
        {
            DisposeAnClear();
        }

        private readonly List<IDisposable> _items = new();
    }
}