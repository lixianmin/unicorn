/*********************************************************************
created:    2022-12-12
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
        public class Snapshot : IEnumerable<TValue>
		{
			internal TValue[] _values;

			internal int _size;

			internal int _version;

			public TValue this[int index] => _values[index];

			public int Count => _size;

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return _GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _GetEnumerator();
			}

			public int GetVersion()
			{
				return _version;
			}

			public bool Take(SortedTable<TKey, TValue> table)
			{
				if (table != null && _version != table._version)
				{
					var capacity = table._capacity;
					var values = _values;
					if (values == null || values.Length != capacity)
					{
						_values = new TValue[capacity];
					}
					
					Array.Copy(table._values, 0, _values, 0, capacity);
					_version = table._version;
					_size = table._size;
					return true;
				}

				return false;
			}

			public SnapshotEnumerator GetEnumerator()
			{
				return new SnapshotEnumerator(this);
			}

			private IEnumerator<TValue> _GetEnumerator()
			{
				int lastVersion = _version;
				int count = _size;
				var values = _values;
				int i = 0;
				while (true)
				{
					if (i >= count)
					{
						yield break;
					}
					
					yield return values[i];
					if (lastVersion != _version)
					{
						break;
					}
					i++;
				}
				throw new InvalidOperationException("Invalid table version");
			}
		}

		public struct SnapshotEnumerator
		{
			private readonly Snapshot _snapshot;

			private int _index;

			private TValue _value;

			private readonly int _version;

			public TValue Current => _value;

			public SnapshotEnumerator(Snapshot snapshot)
			{
				_snapshot = snapshot;
				_index = 0;
				_value = default;
				_version = _snapshot._version;
			}

			public bool MoveNext()
			{
				if (_version != _snapshot._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
				
				int size = _snapshot._size;
				if (_index < size)
				{
					_value = _snapshot._values[_index];
					_index++;
					return true;
				}
				
				_index = size + 1;
				_value = default;
				return false;
			}

			public void Reset()
			{
				if (_version != _snapshot._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
				
				_index = 0;
				_value = default;
			}
		}
    }
}