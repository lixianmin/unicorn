/********************************************************************
created:    2024-02-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unicorn
{
    public class DisposeDog : Disposable
    {
        public DisposeDog()
        {
            OnDisposing += DisposeAndClear;
        }

        public T Add<T>(T item) where T : IDisposable
        {
            if (item != null)
            {
                _items.Add(item);
            }

            return item;
        }

        public void DisposeAndClear()
        {
            var count = _items.Count;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var item = _items[i] as IDisposable;
                    item?.Dispose();
                }

                _items.Clear();
            }
        }

        private readonly ArrayList _items = new();
    }
}