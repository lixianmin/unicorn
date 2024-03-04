
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unicorn.Collections
{
	partial class SortedTable<TKey, TValue>
	{
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> pair)
        {
            var index = IndexOfKey(pair.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], pair.Value);
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> pair)
        {
            var index = IndexOfKey(pair.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], pair.Value))
            {
                RemoveAt(index);
                return true;
            }
            
            return false;
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (null == array)
            {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                var text = $"ArrayIndex = {arrayIndex}, array.Length={array.Length}";
                throw new ArgumentOutOfRangeException(text);
            }
            
            if (array.Length - arrayIndex < Count)
            {
                var text = $"ArrayIndex = {arrayIndex}, array.Length={array.Length}";
                throw new ArgumentException(text);
            }
            
            for (int i= 0; i < Count; ++i)
            {
                var pair = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                array[arrayIndex + i] = pair;
            }
        }

		void IDictionary.Add (object key, object value)
		{
			this.Add (_ToTKey (key), _ToTValue (value));
		}

		bool IDictionary.Contains (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException (nameof(key));
			}
			return key is TKey && this.ContainsKey ((TKey)((object)key));
		}

		void IDictionary.Remove (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException (nameof(key));
			}
			
			if (key is TKey key1)
			{
				this.Remove (key1);
			}
		}

		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return new Enumerator (this);
		}

		object IDictionary.this [object key]
		{
			get
			{
				if (key is TKey && this.ContainsKey ((TKey)((object)key)))
				{
					return this [_ToTKey (key)];
				}
				return null;
			}
			set => this [_ToTKey (key)] = _ToTValue (value);
		}

		private TKey _ToTKey (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException (nameof(key));
			}
			if (!(key is TKey))
			{
				throw new ArgumentException ("not of type: " + typeof(TKey).ToString (), "key");
			}

			return (TKey)key;
		}

		private TValue _ToTValue (object value)
		{
			if (value == null && !typeof(TValue).IsValueType)
			{
				return default;
			}
			if (!(value is TValue))
			{
				throw new ArgumentException ("not of type: " + typeof(TValue).ToString (), "value");
			}

			return (TValue)value;
		}

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection IDictionary.Keys => Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        ICollection IDictionary.Values => Values;

		void ICollection.CopyTo (Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException (nameof(array));
            }

            if (array.Rank > 1 || array.GetLowerBound (0) != 0)
            {
                throw new ArgumentException ("Array must be zero based and single dimentional", "array");
            }

            var count = this._size;
            for (var i= 0; i< count; ++i)
            {
                var item = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                array.SetValue(item, i + arrayIndex);
            }
        }

		bool IDictionary.IsFixedSize => false;

		bool IDictionary.IsReadOnly => false;

		bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
	}
}