
/*********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn.Collections
{
	partial class SortedTable<TKey, TValue>
	{
        public void InsertByIndex (int index, TKey key, TValue val)
		{
			if (index < 0)
			{
				var message = $"[Insert()] index < 0, index={index}";
				throw new ArgumentOutOfRangeException(message);
			}

			if (_size == _keys.Length)
			{
				_EnsureCapacity(_size + 1);
			}

			if (index < _size)
			{
				Array.Copy(_keys, index, _keys, index + 1, _size - index);
				Array.Copy(_values, index, _values, index + 1, _size - index);
			}

			_keys[index]= key;
			_values[index] = val;
			++_size;
			++_version;
		}

        public void _Append (TKey key, TValue val)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (_size == _capacity)
			{
				_EnsureCapacity(_size + 1);
			}

			var index = _size;
			_keys[index]= key;
			_values[index] = val;
			++_size;
			++_version;
		}

        public void _Sort ()
		{
			Array.Sort(_keys, _values, 0, _size, _comparer);
		}
	}
}