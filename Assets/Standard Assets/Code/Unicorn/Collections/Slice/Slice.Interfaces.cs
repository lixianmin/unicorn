/********************************************************************
created:    2024-03-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Unicorn.Collections
{
    public partial class Slice<T>
    {
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(Items, 0, array, arrayIndex, Size);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
            {
                throw new ArgumentException("multi rank is not supported");
            }

            try
            {
                Array.Copy(Items, 0, array, arrayIndex, Size);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new InvalidDataException("array type is not match");
            }
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        bool ICollection<T>.IsReadOnly => false;
    }
}