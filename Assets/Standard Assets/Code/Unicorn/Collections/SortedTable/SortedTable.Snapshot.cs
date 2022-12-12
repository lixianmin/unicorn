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
        public struct Snapshot : IEnumerable<TValue>
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

			public void Take(SortedTable<TKey, TValue> table)
			{
				if (table != null && _version != table._version)
				{
					int capacity = table._capacity;
					TValue[] values = _values;
					if (values == null || values.Length != capacity)
					{
						_values = new TValue[capacity];
					}
					
					Array.Copy(table._values, 0, _values, 0, capacity);
					_version = table._version;
					_size = table._size;
				}
			}

			public SnapshotEnumerator GetEnumerator()
			{
				return new SnapshotEnumerator(this);
			}

			private IEnumerator<TValue> _GetEnumerator()
			{
				int lastVersion = _version;
				int count = _size;
				TValue[] values = _values;
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

			private int _version;

			public TValue Current
			{
				get
				{
					return _value;
				}
			}

			public SnapshotEnumerator(Snapshot snapshot)
			{
				_snapshot = snapshot;
				_index = 0;
				_value = default;
				_version = _snapshot._version;
			}

			public bool MoveNext()
			{
				int version = _version;
				Snapshot snapshot = _snapshot;
				if (version != snapshot._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
				Snapshot snapshot2 = _snapshot;
				int size = snapshot2._size;
				if (_index < size)
				{
					Snapshot snapshot3 = _snapshot;
					_value = snapshot3._values[_index];
					_index++;
					return true;
				}
				_index = size + 1;
				_value = default(TValue);
				return false;
			}

			public void Reset()
			{
				int version = _version;
				Snapshot snapshot = _snapshot;
				if (version != snapshot._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
				_index = 0;
				_value = default;
			}
		}
    }
}