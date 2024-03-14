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
        int IList.Add(object item)
        {
            Add((T)item);
            return Count - 1;
        }

        void IList.Insert(int index, object item)
        {
            Insert(index, (T)item);
        }

        void IList.Remove(object item)
        {
            if (!IsCompatibleObject(item))
            {
                return;
            }

            Remove((T)item);
        }

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.IndexOf(object item) => IsCompatibleObject(item) ? IndexOf((T)item) : -1;

        bool IList.Contains(object item) => IsCompatibleObject(item) && Contains((T)item);

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        private static bool IsCompatibleObject(object value)
        {
            if (value is T)
            {
                return true;
            }

            return value == null && default(T) == null;
        }

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
            catch (ArrayTypeMismatchException)
            {
                throw new InvalidDataException("array type is not match");
            }
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        bool ICollection<T>.IsReadOnly => false;
    }
}