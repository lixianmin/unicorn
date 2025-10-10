/********************************************************************
created:    2025-10-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public struct Chunk<T>
    {
        public T[] Items;
        public int Size;

        public Span<T> AsSpan()
        {
            return Items.AsSpan(0, Size);
        }

        public override string ToString()
        {
            return $"Items.Length={Items?.Length}, Size={Size}";
        }
    }
}