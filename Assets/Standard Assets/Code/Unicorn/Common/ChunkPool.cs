/********************************************************************
created:    2025-10-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Buffers;

namespace Unicorn
{
    public static class ChuckPool
    {
        public static Chunk<T> Get<T>(int size)
        {
            if (size > 0)
            {
                var items = ArrayPool<T>.Shared.Rent(size);
                return new Chunk<T>
                {
                    Items = items,
                    Size = size,
                };
            }

            return default;
        }

        public static void Return<T>(Chunk<T> chunk, bool clearArray = false)
        {
            var items = chunk.Items;
            var length = items?.Length ?? 0;
            if (length > 0 && (length & (length - 1)) == 0)
            {
                ArrayPool<T>.Shared.Return(items, clearArray);
            }
        }
    }
}